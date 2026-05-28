using MassTransit;
using Scalar.AspNetCore;
using UserService.Repositories;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Services;
using NLog;
using NLog.Web;

var logger = LogManager.Setup()
    .LoadConfigurationFromFile("NLog.config")
    .GetCurrentClassLogger();

try
{
    logger.Debug("Starting UserService");

    BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    logger.Info("UserService builder created");

    logger.Info("JWT key configured: {Configured}", !string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Key"]));

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
    builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<UserService.Consumers.UserRegisteredConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
            cfg.ConfigureEndpoints(context);
        });
    });

    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "";
    var jwtSecret = builder.Configuration["Jwt:Key"] ?? "";

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    logger.Warn(context.Exception, "JWT authentication failed in UserService");
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    logger.Info("UserService started successfully");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "UserService stopped because of an exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}