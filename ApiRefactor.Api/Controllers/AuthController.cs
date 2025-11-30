using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiRefactor.Api.Controllers;

[ApiController]
[Route("auth")]
/// <summary>
/// Controller for handling authentication-related actions.
/// </summary>
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config)
    {
        _config = config;
    }
    /// <summary>
    /// Represents a request for user login.
    /// </summary>
    /// <param name="Username"></param>
    /// <param name="Password"></param>
    public record LoginRequest(string Username, string Password);
    /// <summary>
    /// Generates a JWT token for the user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("token")]
    public IActionResult Token([FromBody] LoginRequest request)
    {
        // Simple dev-only validation
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Invalid credentials");

        // In production, validate against user store
        var jwtSection = _config.GetSection("Jwt");
        var issuer = jwtSection["Issuer"] ?? "ApiRefactor";
        var audience = jwtSection["Audience"] ?? "ApiRefactorClients";
        var signingKey = jwtSection["SigningKey"] ?? "ThisIsADevOnlySigningKey_ChangeInProduction_0123456789";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { access_token = tokenString });
    }
}
