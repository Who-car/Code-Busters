using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using CodeBusters.Utils;
using Microsoft.IdentityModel.Tokens;

namespace MyAspHelper.AuthSchemas;

public static class JwtHelper<T> where T: IAuthorizable
{
    public static string GenerateToken(T instance)
    {
        var jwtSecret = App.Settings["JwtSecret"];

        if (jwtSecret is null)
            throw new NullReferenceException("JWT secret not set");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", instance.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(12),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public static Guid ValidateToken(string token)
    {
        var jwtSecret = App.Settings["JwtSecret"];
        
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