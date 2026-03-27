namespace HubAdminPanel.Core.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a user's public profile and account status.
    /// Used for transmitting user data to the client-side while hiding sensitive information.
    /// </summary>
    /// <remarks>
    /// This DTO simplifies the complex <see cref="Entities.User"/> entity by flattening 
    /// role relationships into a simple list of strings.
    /// </remarks>
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new(); 
    }
}