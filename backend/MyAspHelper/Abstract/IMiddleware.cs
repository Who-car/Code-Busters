using System.Net;

namespace MyAspHelper.Abstract.IMiddleware;

public interface IMiddleware
{
    public IMiddleware Next { get; set; }
    public Task Handle(HttpContextResult context);
}