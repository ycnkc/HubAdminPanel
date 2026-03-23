namespace ToDoApi.Models
{
    /// <summary>
    /// Model used for handling token refresh requests.
    /// </summary>
    public class RefreshTokenRequest
    {
        public string? RefreshToken { get; set; }
    }
}