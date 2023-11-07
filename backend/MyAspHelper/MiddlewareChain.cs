using System.Net;

namespace MyAspHelper;

public class MiddlewareChain
{
    private IMiddleware _chainStart;

    public void ResolveContainer(Container container)
    {
        var chain = container.ResolveAll<IMiddleware>();
        for (var i = 0; i < chain.Count-1; i++)
            chain[i].Next = chain[i + 1];
        _chainStart = chain.First();
    }
    
    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        await _chainStart.Handle(request, response);
    }
}