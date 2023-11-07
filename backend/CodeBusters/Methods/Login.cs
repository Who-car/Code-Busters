using System.Net;
using System.Text;
using System.Text.Json;
using CodeBusters.Database;
using CodeBusters.Models;
using CodeBusters.Utils;
using MyAspHelper.Attributes;

namespace CodeBusters.Methods;

public static partial class Methods
{
    [Route("/login")]
    public static async Task LoginAsync(HttpListenerRequest request, HttpListenerResponse response)
    {
        using var sr = new StreamReader(request.InputStream);
        var loginStr = await sr.ReadToEndAsync().ConfigureAwait(false);
        
        var body = new Response();
        
        var login = JsonSerializer.Deserialize<LoginModel>(loginStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (login != null)
        {
            var dbContext = new DbContext();
            var user = new User();
            try
            {
                user = await dbContext.GetUserByEmailAsync(login.Email);
            }
            catch (KeyNotFoundException e)
            {
                body.Success = false;
                body.ErrorInfo.Add("No user with such email");
                body.ErrorInfo.Add($"Additional info: {e.Message}");
                response.ContentType = "application/json";
                response.StatusCode = 404;
                await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
                response.OutputStream.Close();
                return;
            }
        
            if (!Hasher.Validate(user.Password, login.Password))
            {
                body.Success = false;
                body.ErrorInfo.Add("Passwords don't match");
                response.ContentType = "application/json";
                response.StatusCode = 400;
                await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
                response.OutputStream.Close();
                return;
            }
            
            var token = JwtHelper.GenerateToken(user);
        
            body.Success = true;
            body.Token = token;
            response.ContentType = "application/json";
            response.StatusCode = 200;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
            response.OutputStream.Close();
        }
    }
}