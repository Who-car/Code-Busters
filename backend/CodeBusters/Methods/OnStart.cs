using System.Text.Json.Nodes;
using CodeBusters.Database;
using CodeBusters.Models;
using MyOrmHelper;

namespace CodeBusters.Methods;

public static partial class Methods
{
    public static async Task OnStart()
    {
        var db = new DbContext();

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
            new Column("questions", typeof(JsonArray))
        });

        // await db.CreateTableAsync<Question>(tableName: "questions", new[]
        // {
        //     new Column("id", typeof(long), true),
        //     new Column("question", typeof(string)),
        //     new Column("quiz id", typeof(Guid), "quizzes", "id")
        // });
        //
        // await db.CreateTableAsync<Answer>(tableName: "answers", new[]
        // {
        //     new Column("id", typeof(long), true),
        //     new Column("answer", typeof(string)),
        //     new Column("is correct", typeof(bool)),
        //     new Column("question id", typeof(long), "questions", "id")
        // });

        // await db.CreateTableAsync<Tag>(tableName: "tags", new[]
        // {
        //     new Column("id", typeof(int), true),
        //     new Column("label", typeof(string)),
        //     new Column("quiz id", typeof(Guid), "quizzes", "id")
        // });

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