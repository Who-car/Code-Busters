using CodeBusters.Controllers;
using MyAspHelper.Utils;

//TODO: Расставить комментарии адекватные

var builder = new AppBuilder();
var app = builder
    .WithRouteMapping()
    .WithCorsPolicy()
    .WithHttpMethodCheck()
    .WithExceptionHandlers()
    .WithAuthorization()
    .WithPreStart(StartUp.OnStart)
    .Build();

app.Start();