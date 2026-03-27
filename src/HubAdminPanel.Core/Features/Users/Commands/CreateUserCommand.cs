using MediatR;

/// <summary>
/// Command object for registering a new user in the application.
/// Once processed, it returns the unique identifier of the newly created user.
/// </summary>
/// <remarks>
/// This command acts as a Data Transfer Object (DTO) between the API layer 
/// and the <see cref="CreateUserCommandHandler"/>.
/// </remarks>
public class CreateUserCommand : IRequest<int>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<int> RoleIds { get; set; } = new(); // Kullanıcıya hangi roller verilecek?
}