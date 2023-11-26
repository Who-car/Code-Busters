using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using MyAspHelper.Abstract;
using MyAspHelper.Utils;

namespace MyAspHelper;
//TODO: Проверить, что в проекте есть методы не в контроллерах (тогда создавать новый абстрактный контроллер не понадобится)
public class AttributeMapMiddleware : IMiddleware
{
    public IMiddleware? Next { get; set; }
    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var (targetMethod, parameters, controllerType) = request.GetEndpoint();
        if (Next is not null && controllerType is null) await Next.Handle(request, response);
        
        var controllerExpr = Expression.MemberInit(Expression.New(controllerType!));
        var controller = Expression.Lambda<Func<Controller>>(controllerExpr).Compile()();
        controller.Request = request;
        controller.Response = response;
        
        await (Task)targetMethod!.Invoke(controller, parameters)!;
        
        if (Next is not null) await Next.Handle(request, response);
    }
}