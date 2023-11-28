using System.Text.RegularExpressions;
using MyAspHelper.Abstract;
using MyAspHelper.Abstract.IMiddleware;
using MyAspHelper.Middlewares;

namespace MyAspHelper.Utils;

public class AppBuilder
{
    private readonly App _app = new();

    public AppBuilder WithAuthorization()
    {
        _app.IocContainer.Register<IMiddleware, AuthMiddleware>();
        return this;
    }

    public AppBuilder WithRouteMapping()
    {
        _app.IocContainer.Register<IMiddleware, MapMiddleware>();
        return this;
    }

    public AppBuilder WithCorsPolicy()
    {
        _app.IocContainer.Register<IMiddleware, CorsConfigMiddleware>();
        return this;
    }
    
    public AppBuilder WithHttpMethodCheck()
    {
        _app.IocContainer.Register<IMiddleware, HttpMethodMiddleware>();
        return this;
    }
    
    public AppBuilder WithExceptionHandlers()
    {
        _app.IocContainer.Register<IMiddleware, ExceptionHandlerMiddleware>();
        return this;
    }
    
    public AppBuilder WithDbConfiguration<TRepository>(string? connectionString = null) where TRepository : IRepository
    {
        connectionString ??= ConnectionString.Get(App.Settings["DatabaseConnectionString"]);
        TRepository.ConfigureDb(connectionString);
        return this;
    }
    
    public AppBuilder WithPreStart(Func<Task> func)
    {
        func.Invoke();
        return this;
    }

    public App Build()
    {
        _app.IocContainer.Register<IMiddleware, FinalMiddleware>();
        _app.ResolveDependencies();
        return _app;
    }
}