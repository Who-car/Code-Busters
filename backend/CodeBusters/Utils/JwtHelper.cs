using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using CodeBusters.Models;
using Microsoft.IdentityModel.Tokens;

namespace CodeBusters.Utils;

public static class JwtHelper
{
    public static string GenerateToken(User user)
    {
        using var jsonReader = new StreamReader("../../../appsettings.json");
        var jwtSecret = JsonNode.Parse(jsonReader.ReadToEnd())!["JwtSecret"]?.GetValue<string>();

        if (jwtSecret is null)
            throw new NullReferenceException("JWT secret not set");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public static Guid ValidateToken(string token)
    {
        using var jsonReader = new StreamReader("../../../appsettings.json");
        var jwtSecret = JsonNode.Parse(jsonReader.ReadToEnd())!["JwtSecret"]?.GetValue<string>();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSecret!);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            
            return userId;
        }
        catch
        {
            return Guid.Empty;
        }
    }
}