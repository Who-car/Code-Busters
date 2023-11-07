using System.Net;
using System.Reflection;
using MyAspHelper.Attributes;

namespace MyAspHelper;

public static class HttpListenerRequestExtension
{
    private static List<MethodInfo>? _methods;

    public static MethodInfo? GetEndpoint(this HttpListenerRequest request)
    {
        _methods ??= Assembly
            .GetEntryAssembly()!
            .GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttributes(typeof(RouteAttribute), false).Length > 0)
            .ToList();
        
        var requestPath = request.Url!.LocalPath.Replace("api/", "").ToLower();
        
        return _methods
            .FirstOrDefault(m => 
                m.GetCustomAttribute<RouteAttribute>()?.Path.ToLower() == requestPath);
    }
}