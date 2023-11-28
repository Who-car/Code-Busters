using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MyAspHelper.Abstract;
using MyAspHelper.Middlewares;
using MyAspHelper.Utils;

namespace MyAspHelper;

public class App
{
    private readonly MiddlewareChain _chain = new();
    private readonly HttpListener _listener = new(); 
    public readonly IocContainer IocContainer = new();
    public static readonly AppSettings Settings = new();

    internal void ResolveDependencies()
    {
        _chain.ResolveContainer(IocContainer);
        //Здесь также можно резолвить любые зависимости,
        //которые могут понадобится в работе проекта
    }

    private async Task HandleIncomingRequests()
    { 
        while (_listener.IsListening)
        {
            var context = new HttpContextResult(await _listener.GetContextAsync());

            await _chain.Handle(context);
        }
    }

    public void Start()
    {
        var url = Settings["ApplicationUrl"];
        if (url is null) 
            throw new ArgumentNullException($"Url not set");
        
        _listener.Prefixes.Add(url);
        _listener.Start();
        Console.WriteLine($"Server started\nListening on {url}...");

        var listenTask = HandleIncomingRequests();
        listenTask.GetAwaiter().GetResult();
        
        _listener.Close();
    }
}