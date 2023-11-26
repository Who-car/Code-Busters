using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MyAspHelper;

public class App
{
    private readonly MiddlewareChain _chain = new();
    private readonly HttpListener _listener = new(); 
    public readonly IocContainer IocContainer = new();

    private async Task HandleIncomingRequests()
    { 
        while (_listener.IsListening)
        {
            var context = await _listener.GetContextAsync();
            var req = context.Request;
            var rep = context.Response;

            await _chain.Handle(req, rep);
        }
    }

    private static string GetUrl()
    {
        using var jsonReader = new StreamReader("../../../appsettings.json");
        var json = jsonReader.ReadToEnd();
        var options = JsonNode.Parse(json);

        var urlInfo = options!["ApplicationUrl"];
        if (urlInfo is null || urlInfo.GetValue<string>() is not string url)
            throw new ArgumentNullException("Url not set");

        return url;
    }

    public void Start()
    {
        var url = GetUrl();
        
        _chain.ResolveContainer(IocContainer);
        _listener.Prefixes.Add(url);
        _listener.Start();
        Console.WriteLine($"Server started\nListening on {url}...");

        var listenTask = HandleIncomingRequests();
        listenTask.GetAwaiter().GetResult();
        
        _listener.Close();
    }
}