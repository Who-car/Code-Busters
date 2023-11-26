using System.Net;
using MyAspHelper.Attributes;
using MyAspHelper.Utils;

namespace MyAspHelper;

public class AuthMiddleware : IMiddleware
{
    public IMiddleware? Next { get; set; }
    
    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var (targetMethod, _, _) = request.GetEndpoint();

        if (targetMethod is not null && targetMethod.GetCustomAttributes(typeof(AuthorizeAttribute), false).Length > 0)
        {
            if (request.Headers["authToken"] is null)
            {
                response.StatusCode = 401;
                response.OutputStream.Close();
                return;
            }
        }

        if (Next is not null) await Next.Handle(request, response);
    }
}