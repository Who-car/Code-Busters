using System.Net;
using MyAspHelper.Abstract;
using MyAspHelper.Abstract.IMiddleware;

namespace MyAspHelper.Middlewares;

public class MiddlewareChain
{
    private IMiddleware _chainStart = null!;

    public void ResolveContainer(IocContainer iocContainer)
    {
        var chain = iocContainer.ResolveAll<IMiddleware>();
        for (var i = 0; i < chain.Count-1; i++)
            chain[i].Next = chain[i + 1];
        _chainStart = chain.First();
    }
    
    public async Task Handle(HttpContextResult context)
    {
        await _chainStart.Handle(context);
    }
}