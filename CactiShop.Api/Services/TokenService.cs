using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CactiShop.Api.Data.Models;
using Microsoft.IdentityModel.Tokens;

namespace CactiShop.Api.Services;

public class TokenService(IConfiguration config)
{
    public string CreateToken(User user)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Role, user.Role.Name)
    };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
