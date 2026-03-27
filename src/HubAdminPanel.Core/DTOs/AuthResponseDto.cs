namespace HubAdminPanel.Core.DTOs
{
    /// <summary>
    /// Data Transfer Object containing security tokens issued after a successful authentication.
    /// Provides the necessary credentials for accessing protected resources and managing sessions.
    /// </summary>
    /// <remarks>
    /// This object is typically returned by Login and Refresh Token operations.
    /// </remarks>
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
