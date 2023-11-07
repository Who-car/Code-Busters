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
    [Route("/register")]
    public static async Task RegisterAsync(HttpListenerRequest request, HttpListenerResponse response)
    {
        using var sr = new StreamReader(request.InputStream);
        var userStr = await sr.ReadToEndAsync().ConfigureAwait(false);
        var body = new Response();
        
        var user = JsonSerializer.Deserialize<User>(userStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (user != null)
        {
            var validationResult = CustomValidator.Validate(user);
            
            if (validationResult is { isSuccess: false })
            {
                body.Success = false;
                body.ErrorInfo = validationResult.results.Select(er => er.ErrorMessage).ToList();
                response.ContentType = "application/json";
                response.StatusCode = 400;
                await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
                response.OutputStream.Close();
                return;
            }
            
            user.Password = Hasher.Hash(user.Password);
        
            var dbContext = new DbContext();
        
            if (await dbContext.CheckUserExists(user))
            {
                body.Success = false;
                body.ErrorInfo.Add("user already exists");
                response.ContentType = "application/json";
                response.StatusCode = 400;
                await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
                response.OutputStream.Close();
                return;
            }
            user.Id = Guid.NewGuid();
            await dbContext.AddNewUserAsync(user);
        
            body.Success = true;
            body.Text = "added user successfully";
            response.ContentType = "application/json; charset=utf-8";
            response.StatusCode = 200;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
            response.OutputStream.Close();
        }
    }
}