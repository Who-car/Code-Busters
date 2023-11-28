using System.Diagnostics;
using CodeBusters.Controllers;
using CodeBusters.Repository;
using MyAspHelper.Middlewares;
using MyAspHelper.Utils;

// var mw = new MapMiddleware();
// Benchmark.TestTask(10000, () =>
// {
//     mw.GetEndpoint("api/User/00000000-0000-0000-0000-000000000000/get");
// });

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