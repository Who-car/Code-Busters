using CodeBusters.Methods;
using MyAspHelper;

var builder = new AppBuilder();
var app = builder
    .WithCorsPolicy()
    .WithApiCheck()
    .WithAuthorization()
    .WithAttributeRoutes()
    .WithPreStart(Methods.OnStart)
    .Build();

app.Start();