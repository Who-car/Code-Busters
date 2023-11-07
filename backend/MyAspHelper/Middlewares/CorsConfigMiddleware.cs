using System.Net;

namespace MyAspHelper;

public class CorsConfigMiddleware : IMiddleware
{
    public IMiddleware? Next { get; set; }
    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        response.AppendHeader("Access-Control-Allow-Origin", "*");
        response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
        response.AddHeader("Content-type", "application/json");
        response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
        response.AddHeader("Access-Control-Max-Age", "1728000");
        
        if (request.HttpMethod == "OPTIONS") response.OutputStream.Close();
        
        else if (Next is not null) await Next.Handle(request, response);
    }
}