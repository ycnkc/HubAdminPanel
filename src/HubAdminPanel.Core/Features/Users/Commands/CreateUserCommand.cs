using MediatR;

public class CreateUserCommand : IRequest<int>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<int> RoleIds { get; set; } = new(); // Kullanıcıya hangi roller verilecek?
}