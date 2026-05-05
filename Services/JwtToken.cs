using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtToken
{
    private readonly IConfiguration _config;

    public JwtToken(IConfiguration config)
    {
        _config = config;

    }

    public string CreateToken(string userId, string email)
    {
        var keyString = _config["Jwt:Key"]
            ?? throw new Exception("Jwt:Key is missing in appsettings.json.");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(keyString)
            );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email)
            };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}