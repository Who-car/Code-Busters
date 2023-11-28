using System.Text.Json.Nodes;
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

        await db.CreateTableAsync<User>(tableName: "users", new CancellationToken(), new[]
        {
            new Column("id", typeof(Guid), true),
            new Column("name", typeof(string)),
            new Column("email", typeof(string)),
            new Column("password", typeof(string))
        });

        await db.CreateTableAsync<Quiz>(tableName: "quizzes", new CancellationToken(), new[]
        {
            new Column("id", typeof(Guid), true),
            new Column("topic", typeof(string)),
            new Column("author id", typeof(Guid), "users", "id"),
            new Column("difficulty", typeof(Difficulty)),
            new Column("questions", typeof(JsonArray))
        });

        await db.CreateTableAsync<Comment>(tableName: "comments", new CancellationToken(), new[]
        {
            new Column("id", typeof(Guid), true),
            new Column("text", typeof(string)),
            new Column("rating", typeof(double)),
            new Column("author id", typeof(Guid), "users", "id"),
            new Column("quiz id", typeof(Guid), "quizzes", "id")
        });
    }
}