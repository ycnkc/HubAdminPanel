namespace ToDoApi.Models
{
    /// <summary>
    /// Model used for handling token refresh requests.
    /// </summary>
    public class TokenModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}