using MassTransit;
using UserService.DTOs;
using FitLife.Events;
using UserService.Models;
using UserService.Services;

namespace UserService.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IUserService _userService;

    public UserRegisteredConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
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
