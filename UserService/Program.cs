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

// NLog initialiseres før builder, så startup-fejl også logges
var logger = LogManager.Setup()
    .LoadConfigurationFromFile("NLog.config")
    .GetCurrentClassLogger();

try
{
    logger.Debug("Starting UserService");

    // Guid serialiseres som standard UUID-streng i MongoDB frem for det binære CSUUID-format
    BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

    var builder = WebApplication.CreateBuilder(args);

    // Erstat ASP.NET Cores standard-logging med NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    logger.Info("UserService builder created");
    logger.Info("JWT key configured: {Configured}", !string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Key"]));

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // Repository som Singleton — MongoClient er thread-safe og bør genbruges på tværs af requests
    builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
    builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

    // MassTransit med RabbitMQ — registrerer UserRegisteredConsumer så den automatisk
    // lytter på køen og opretter brugerprofiler når AuthService publicerer events
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
            // Konfigurerer kønavne automatisk baseret på consumer-typen
            cfg.ConfigureEndpoints(context);
        });
    });

    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "";
    var jwtSecret = builder.Configuration["Jwt:Key"] ?? "";

    // JWT-validering — UserService udsteder ikke tokens, men validerer dem fra AuthService
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

            // Log JWT-fejl eksplicit så de kan ses i docker logs uden at kaste exceptions
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
    // Rækkefølgen er vigtig: Authentication skal stå før Authorization
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    logger.Info("UserService started successfully");

    app.Run();
}
catch (Exception ex)
{
    // Fanger fejl der sker under opstart — f.eks. manglende MongoDB-forbindelse
    logger.Error(ex, "UserService stopped because of an exception");
    throw;
}
finally
{
    // Sikrer at NLog flushes alle bufferede log-beskeder inden processen lukker
    LogManager.Shutdown();
}