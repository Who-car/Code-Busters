using System.Diagnostics;
using CodeBusters.Controllers;
using CodeBusters.Repository;
using MyAspHelper.Middlewares;
using MyAspHelper.Utils;

// TODO: делать бенчмаркинг не в Programm)
// TODO: Рефакторинг всего кода
// var mw = new MapMiddleware();
// Benchmark.TestTask(10000, () =>
// {
//     mw.GetEndpoint("api/User/00000000-0000-0000-0000-000000000000/get");
// });

var builder = new AppBuilder();
var app = builder
    .WithCorsPolicy()
    .WithRouteMapping()
    .WithHttpMethodCheck()
    .WithExceptionHandlers()
    .WithAuthorization()
    .WithDbConfiguration<DbContext>()
    .WithPreStart(StartUp.OnStart)
    .Build();

app.Start();