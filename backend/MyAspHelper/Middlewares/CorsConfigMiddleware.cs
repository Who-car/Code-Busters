using System.Net;
using MyAspHelper.Abstract;
using MyAspHelper.Abstract.IMiddleware;

namespace MyAspHelper.Middlewares;

public class CorsConfigMiddleware : IMiddleware
{
    public required IMiddleware Next { get; set; }
    public async Task Handle(HttpContextResult context)
    {
        context.Response.AppendHeader("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
        context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
        context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
        context.Response.AddHeader("Content-type", "application/json");
        context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
        context.Response.AddHeader("Access-Control-Max-Age", "1728000");

        if (context.Request.HttpMethod == "OPTIONS") 
            await context.SendResponse(null, null);
        else 
            await Next.Handle(context);
    }
}