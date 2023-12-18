using System.Text.Json.Nodes;
using CodeBusters.DbModels;
using CodeBusters.Models;
using CodeBusters.Repository;
using MyOrmHelper;

namespace CodeBusters.Controllers;

public static class StartUp
{
    public static async Task OnStart()
    {
        var db = new DbContext();

        await db.CreateEnumAsync(typeof(Difficulty), new CancellationToken());
        await db.CreateEnumAsync(typeof(Status), new CancellationToken());

        await db.CreateTableAsync(tableName: "users", new CancellationToken(), new[]
        {
            new Column("id", typeof(Guid), true),
            new Column("name", typeof(string)),
            new Column("email", typeof(string)),
            new Column("password", typeof(string))
        });

        await db.CreateTableAsync(tableName: "quizzes", new CancellationToken(), new[]
        {
            new Column("id", typeof(Guid), true),
            new Column("topic", typeof(string)),
            new Column("author", typeof(string)),
            new Column("difficulty", typeof(Difficulty)),
            new Column("description", typeof(string)),
            new Column("questions", typeof(JsonArray))
        });

        await db.CreateTableAsync(tableName: "comments", new CancellationToken(), new[]
        {
            new Column("id", typeof(Guid), true),
            new Column("text", typeof(string)),
            new Column("rating", typeof(double)),
            new Column("author id", typeof(Guid), "users", "id"),
            new Column("quiz id", typeof(Guid), "quizzes", "id")
        });

        await db.CreateTableAsync(tableName: "friends", new CancellationToken(), new[]
        {
            new Column("id", typeof(Guid), true),
            new Column("user a", typeof(Guid)),
            new Column("user b", typeof(Guid)),
            new Column("status", typeof(Status))
        });
    }
}