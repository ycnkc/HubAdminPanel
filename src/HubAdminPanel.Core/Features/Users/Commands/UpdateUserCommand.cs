using MediatR;

public class UpdateUserCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<int> RoleIds { get; set; } = new();
}