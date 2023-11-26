using System.Net;
using System.Text;

namespace MyAspHelper;

public class ApiCheckMiddleware : IMiddleware
{
    public IMiddleware? Next { get; set; }

    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.Url == null || !request.Url.LocalPath.StartsWith("/api"))
        {
            response.StatusCode = 404;
            await response.OutputStream.WriteAsync("No corresponding method found"u8.ToArray());
            response.OutputStream.Close();
            return;
        }
        
        if (Next is not null) await Next.Handle(request, response);
    }
}