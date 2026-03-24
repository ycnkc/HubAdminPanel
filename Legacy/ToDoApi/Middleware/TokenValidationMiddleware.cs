using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        if (path.Contains("/login") || path.Contains("/register") || path.Contains("/refresh"))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader?.StartsWith("Bearer ") == true ? authHeader.Split(" ").Last() : null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                ValidateToken(token);
            }
            catch (SecurityTokenExpiredException)
            {
                await HandleExceptionAsync(context, "Token has expired. Please refresh your token.", HttpStatusCode.Unauthorized);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token Validation Error: {ex.Message}");
                await HandleExceptionAsync(context, "Invalid token.", HttpStatusCode.Unauthorized);
                return;
            }
        }

        await _next(context);
    }

    private void ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new Exception("JWT Key is missing in appsettings.json!");
        }

        var key = Encoding.UTF8.GetBytes(jwtKey);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero 
        }, out SecurityToken validatedToken);
    }

    private static Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode code)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsJsonAsync(new { error = message });
    }
}