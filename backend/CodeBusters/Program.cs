using CodeBusters.Controllers;
using CodeBusters.Repository;
using MyAspHelper.Utils;

//TODO: Расставить комментарии адекватные

var builder = new AppBuilder();
var app = builder
    .WithRouteMapping()
    .WithCorsPolicy()
    .WithHttpMethodCheck()
    .WithExceptionHandlers()
    .WithAuthorization()
    .WithDbConfiguration<DbContext>()
    .WithPreStart(StartUp.OnStart)
    .Build();

app.Start();