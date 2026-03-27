namespace HubAdminPanel.Core.DTOs
{
    /// <summary>
    /// Data Transfer Object used for user registration requests.
    /// Carries the necessary credentials from the client to the registration logic.
    /// </summary>
    /// <remarks>
    /// This DTO is typically used by the <see cref="Features.Auth.Commands.RegisterUserCommand"/> 
    /// to facilitate the account creation process.
    /// </remarks>
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
