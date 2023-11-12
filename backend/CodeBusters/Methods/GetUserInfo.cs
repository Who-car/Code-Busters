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
    [Authorize]
    [Route("/getUserInfo")]
    public static async Task GetUserInfoAsync(HttpListenerRequest request, HttpListenerResponse response)
    {
        var cancellationToken = new CancellationToken();
        var token = request.Headers["authToken"]!;
        var userId = JwtHelper<User>.ValidateToken(token);
        var body = new Response();

        if (userId == Guid.Empty)
        {
            response.StatusCode = 403;
            body.Success = false;
            body.ErrorInfo.Add("invalid token");
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)), cancellationToken);
            response.OutputStream.Close();
            return;
        }

        var db = new DbContext();
        var user = await db.GetUserByIdAsync(userId, cancellationToken);

        response.StatusCode = 200;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user)), cancellationToken);
        response.OutputStream.Close();
    }
}