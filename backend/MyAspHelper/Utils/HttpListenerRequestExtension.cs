using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using MyAspHelper.Abstract;
using MyAspHelper.Attributes;

namespace MyAspHelper.Utils;

public static class HttpListenerRequestExtension
{
    private static Dictionary<Type, MethodInfo[]>? _controllers;

    public static (MethodInfo? method, object[] parameters, Type? controllerType) 
        GetEndpoint(this HttpListenerRequest request)
    {
        _controllers ??= Assembly
            .GetEntryAssembly()!
            .GetExportedTypes()
            .Where(t => t.BaseType == typeof(Controller))
            .ToDictionary(
                k => k, 
                v => v
                    .GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(RouteAttribute), false).Length > 0)
                    .ToArray());

        var parts = request.Url!.LocalPath.Split('/');
        
        var controller = 
            _controllers
                .FirstOrDefault(t => 
                    parts.Any(p => 
                        t.Key.Name.Replace("Controller", "").Equals(p)));

        MethodInfo? targetMethod = null;
        List<object> parameters = new();
        Type? instance = null;

        if (controller.Equals(default(KeyValuePair<Type, MethodInfo[]>)))
            return (targetMethod, parameters.ToArray(), instance);
        
        foreach (var methodInfo in controller.Value)
        {
            var mPath = methodInfo.GetCustomAttribute<RouteAttribute>()!.Path;
            var mParts = mPath.Split('/');
            var isMatch = true;
                
            if (parts.Length != mParts.Length)
                continue;

            for (var i = 0; i < parts.Length; i++)
            {
                if (Regex.IsMatch(mParts[i], @"\{[a-zA-Z]+:[a-zA-Z]+\}"))
                {
                    var value = parts[i];
                    var type = mParts[i].Trim('{', '}').Split(':')[1];
                        
                    TryParse(type, value, out var parsed);

                    if (parsed is null)
                        throw new ArgumentException($"Could not parse value {value} to the type {type}");
                    parameters.Add(parsed);
                }
                else if (Regex.IsMatch(mParts[i], @"\{([^}]*)\}"))
                {
                    var value = parts[i];
                    parameters.Add(value);
                }
                else
                {
                    isMatch = mParts[i].Equals(parts[i]);
                    if (!isMatch) break;
                }
            }
                
            if (!isMatch) continue;
            targetMethod = methodInfo;
            break;
        }

        return (targetMethod, parameters.ToArray(), controller.Key);
    }

    private static void TryParse(string type, string value, out object? parsed)
    {
        parsed = type switch
        {
            "guid" => Guid.TryParse(value, out var g) ? g : null,
            "int" => int.TryParse(value, out var i) ? i : null,
            _ => value
        };
    }
}