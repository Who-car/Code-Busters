using System.Net;
using MyAspHelper.Abstract;
using MyAspHelper.Abstract.IMiddleware;
using MyAspHelper.Attributes;
using MyAspHelper.Utils;

namespace MyAspHelper.Middlewares;

public class AuthMiddleware : IMiddleware
{
    // Из официальной документации Microsoft:
    // Модификатор required указывает, что поле или свойство, к которому они применены,
    // должны быть инициализированы инициализатором объекта. Любое выражение, которое
    // инициализирует новый экземпляр типа, должно инициализировать все необходимые
    // элементы. Модификатор required доступен начиная с C# 11. Модификатор required
    // позволяет разработчикам создавать типы, в которых свойства или поля должны быть
    // инициализированы должным образом, но при этом разрешать инициализацию с помощью
    // инициализаторов объектов. 
    // https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/keywords/required
    
    // Здесь используется, чтобы игнорировать предупреждения компилятора
    public required IMiddleware Next { get; set; }
    
    //TODO: токен должен храниться в cookies
    public async Task Handle(HttpContextResult context)
    {
        Console.WriteLine();
        if (context.TargetMethod!.GetCustomAttributes(typeof(AuthorizeAttribute), false).Length > 0)
        {
            if (context.AuthToken is null)
            {
                await context.SendResponse(401, "Unauthorized");
                return;
            }
        }

        await Next.Handle(context);
    }
}