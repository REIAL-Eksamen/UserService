using MassTransit;
using UserService.DTOs;
using FitLife.Events;
using UserService.Models;
using UserService.Services;

namespace UserService.Consumers;

/// <summary>
/// MassTransit-consumer der lytter på <see cref="UserRegisteredEvent"/> fra RabbitMQ.
/// Opretter automatisk en brugerprofil i UserDB, når AuthService registrerer en ny bruger.
/// Dette holder de to services løst koblede — UserService behøver ikke kende til AuthService.
/// </summary>
public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IUserService _userService;

    public UserRegisteredConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        // Membership og MembershipStatus sendes som strings i eventet (enums deles ikke på tværs af services)
        // og parses her til de lokale enum-typer
        var dto = new CreateUserDto
        {
            AuthId = context.Message.AuthId,
            Email = context.Message.Email,
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
            PhoneNumber = context.Message.PhoneNumber,
            Membership = Enum.Parse<MembershipType>(context.Message.Membership),
            MembershipStatus = Enum.Parse<MembershipStatus>(context.Message.MembershipStatus)
        };

        _userService.Create(dto);

        return Task.CompletedTask;
    }
}
