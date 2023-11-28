using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using MyAspHelper.Abstract;
using MyAspHelper.Abstract.IMiddleware;
using MyAspHelper.Attributes;
using MyAspHelper.Utils;

namespace MyAspHelper.Middlewares;
//TODO: Причесать эту срань господню (Кисловка говорит что LINQ это слишком медленно)
public class MapMiddleware : IMiddleware
{
    private static Dictionary<Type, MethodInfo[]>? _controllers;
    public required IMiddleware Next { get; set; }
    public async Task Handle(HttpContextResult context)
    {
        if (context.TargetMethod == null
            && context.Controller == null)
            await GetEndpoint(context);
        
        await Next.Handle(context);
    }
    
    
    private async Task GetEndpoint(HttpContextResult context)
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

        var parts = context.Request.Url!.LocalPath
            .Split('/')
            .Where(p => !string.IsNullOrEmpty(p))
            .ToArray();
        
        var controller = 
            _controllers
                .FirstOrDefault(t => 
                    parts.Any(p => 
                        t.Key.Name.Replace("Controller", "").Equals(p)));

        MethodInfo? targetMethod = null;
        List<object> parameters = new();
        Type? instance = null;

        if (controller.Equals(default(KeyValuePair<Type, MethodInfo[]>)))
            return;
        
        foreach (var methodInfo in controller.Value)
        {
            var mPath = methodInfo.GetCustomAttribute<RouteAttribute>()!.Path;
            var mParts = mPath
                .Split('/')
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
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
                    if (!isMatch)
                    {
                        parameters.Clear();
                        break;
                    }
                }
            }
                
            if (!isMatch) continue;
            targetMethod = methodInfo;
            break;
        }

        if (targetMethod == null)
        {
            await context.SendResponse(404, "No corresponding method found");
            return;
        }
        
        var controllerExpr = Expression.MemberInit(Expression.New(controller.Key));
        var targetController = Expression.Lambda<Func<Controller>>(controllerExpr).Compile()();
        targetController.Request = context.Request;
        targetController.Response = context.Response;
        context.TargetMethod = targetMethod;
        context.Parameters = parameters.ToArray();
        context.Controller = targetController;
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