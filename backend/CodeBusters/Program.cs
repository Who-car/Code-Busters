using CodeBusters.Methods;
using CodeBusters.Models;
using MyAspHelper;
using MyOrmHelper;

//TODO: Расставить комментарии адекватные

var builder = new AppBuilder();
var app = builder
    .WithCorsPolicy()
    .WithApiCheck()
    .WithAuthorization()
    .WithAttributeRoutes()
    .WithPreStart(Methods.OnStart)
    .Build();

app.Start();