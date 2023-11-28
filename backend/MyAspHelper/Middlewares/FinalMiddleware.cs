using MyAspHelper.Abstract;
using MyAspHelper.Abstract.IMiddleware;

namespace MyAspHelper.Middlewares;

public class FinalMiddleware : IMiddleware
{
    public IMiddleware Next { get; set; }

    public async Task Handle(HttpContextResult context)
    {
        await context.SendResponse(null, null);
    }
}