using System.Net;

namespace MyAspHelper;

public class ApiCheckMiddleware : IMiddleware
{
    public IMiddleware? Next { get; set; }

    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.Url == null || !request.Url.LocalPath.StartsWith("/api"))
        {
            response.StatusCode = 404;
            response.OutputStream.Close();
            return;
        }
        
        if (Next is not null) await Next.Handle(request, response);
    }
}