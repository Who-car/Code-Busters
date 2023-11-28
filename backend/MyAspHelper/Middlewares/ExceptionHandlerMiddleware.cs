using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using MyAspHelper.Abstract;
using MyAspHelper.Abstract.IMiddleware;

namespace MyAspHelper.Middlewares;

public class ExceptionHandlerMiddleware : IMiddleware
{
    public required IMiddleware Next { get; set; }
    public async Task Handle(HttpContextResult context)
    {
        try
        {
            await (Task)context.TargetMethod!.Invoke(context.Controller, context.Parameters)!;
            await Next.Handle(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await context.SendResponse(500, $"Internal server error.\nAdditional info: {e.Message}");
        }
    }
}