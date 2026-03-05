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
        var path = context.Request.Path.Value?.ToLower();
        if (path.EndsWith("/login") || path.EndsWith("/register") || path.EndsWith("/refresh"))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
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
            catch (Exception)
            {
                await HandleExceptionAsync(context, "Invalid token.", HttpStatusCode.Unauthorized);
                return;
            }
        }

        await _next(context);
    }

    private void ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
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