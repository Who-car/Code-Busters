using System.Net;

namespace MyAspHelper;

public interface IMiddleware
{
    public IMiddleware? Next { get; set; }
    public Task Handle(HttpListenerRequest request, HttpListenerResponse response);
}