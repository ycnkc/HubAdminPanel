public class LoginRequest
{
    /// <summary>
    /// Your username.
    /// </summary>
    /// <example>yaren</example>
    public string Username { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    /// <example>password</example>
    public string PasswordHash { get; set; }
}