using CodeBusters.Methods;
using CodeBusters.Models;
using MyAspHelper;
using MyOrmHelper;

var builder = new AppBuilder();
var app = builder
    .WithCorsPolicy()
    .WithApiCheck()
    .WithAuthorization()
    .WithAttributeRoutes()
    .WithPreStart(Methods.OnStart)
    .Build();

app.Start();