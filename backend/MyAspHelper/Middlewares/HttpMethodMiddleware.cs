using System.Reflection;
using MyAspHelper.Abstract;
using MyAspHelper.Abstract.IMiddleware;
using MyAspHelper.Attributes.HttpMethods;

namespace MyAspHelper.Middlewares;

public class HttpMethodMiddleware : IMiddleware
{
    public required IMiddleware Next { get; set; }
    public async Task Handle(HttpContextResult context)
    {
        var methodType = context.TargetMethod!.GetCustomAttribute(typeof(HttpBaseMethodAttribute), true);
        
        if (methodType is not null)
        {
            var http = Normalize(methodType.TypeId);
            if (!http.Equals(context.Request.HttpMethod))
            {
                await context.SendResponse(405, "Method not allowed");
                return;
            }
        }

        await Next.Handle(context);
    }

    private string Normalize(object input)
    {
        return input
            .ToString()!
            .Split('.')
            .Last()
            .Replace("Http", "")
            .Replace("Attribute", "")
            .ToUpper();
    }
}