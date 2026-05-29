namespace FitLife.Events;

/// <summary>
/// Event der publiceres til RabbitMQ af AuthService, når en ny bruger registrerer sig.
/// UserService forbruger dette event og opretter en tilsvarende brugerprofil i UserDB.
/// Namespace er delt (<c>FitLife.Events</c>) så MassTransit kan matche publisher og consumer korrekt.
/// </summary>
public class UserRegisteredEvent
{
    public string AuthId { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    /// <summary>Sendes som string da enums ikke deles på tværs af services.</summary>
    public string Membership { get; set; } = "Standard";
    /// <summary>Sendes som string da enums ikke deles på tværs af services.</summary>
    public string MembershipStatus { get; set; } = "Active";
}
