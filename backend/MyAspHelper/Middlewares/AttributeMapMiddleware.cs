using System.Net;

namespace MyAspHelper;

public class AttributeMapMiddleware : IMiddleware
{
    public IMiddleware? Next { get; set; }
    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var targetMethod = request.GetEndpoint();
        
        if (targetMethod != null)
            await (Task)targetMethod.Invoke(null, new object[] { request, response })!;
        
        if (Next is not null) await Next.Handle(request, response);
    }
}