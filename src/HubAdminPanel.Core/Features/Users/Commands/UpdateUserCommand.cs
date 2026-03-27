using MediatR;

/// <summary>
/// Command object for updating an existing user's profile and their assigned roles.
/// Transports data from the API/UI layer to the <see cref="UpdateUserCommandHandler"/>.
/// </summary>
public class UpdateUserCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<int> RoleIds { get; set; } = new();
}