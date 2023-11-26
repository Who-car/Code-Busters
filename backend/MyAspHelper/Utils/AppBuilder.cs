namespace MyAspHelper;

public class AppBuilder
{
    private readonly App _app = new();

    public AppBuilder WithAuthorization()
    {
        _app.IocContainer.Register<IMiddleware, AuthMiddleware>();
        return this;
    }

    public AppBuilder WithApiCheck()
    {
        _app.IocContainer.Register<IMiddleware, ApiCheckMiddleware>();
        return this;
    }

    public AppBuilder WithAttributeRoutes()
    {
        _app.IocContainer.Register<IMiddleware, AttributeMapMiddleware>();
        return this;
    }

    public AppBuilder WithCorsPolicy()
    {
        _app.IocContainer.Register<IMiddleware, CorsConfigMiddleware>();
        return this;
    }
    
    public AppBuilder WithPreStart(Func<Task> func)
    {
        func.Invoke();
        return this;
    }

    public AppBuilder WithPreStart(Func<object[]?, Task> func, object[]? parameters)
    {
        func.Invoke(parameters);
        return this;
    }

    public App Build()
    {
        return _app;
    }
}