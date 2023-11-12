using CodeBusters.Models;
using MyOrmHelper;
using Npgsql;

namespace CodeBusters.Database;

public class DbContext
{
    private const string ConnectionString = "Host=localhost;" +
                                            "Port=5432;" +
                                            "Username=postgres;" +
                                            "Password=ab9q0rui;" +
                                            "Database=CodeBusters;" +
                                            "Include Error Detail = true;";
    private readonly NpgsqlConnection _connection = new(ConnectionString);

    public async Task CreateTableAsync<T>(string tableName, CancellationToken token, Column[] columns)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<T>(_connection);
        await orm.CreateTableAsync(tableName, token, columns);
        await _connection.CloseAsync();
    }

    public async Task AddNewUserAsync(User user, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<User>(_connection);
        await orm.InsertAsync(user, token);
        await _connection.CloseAsync();
    }
    
    public async Task AddNewQuizAsync(Quiz quiz, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var quizOrm = new OrmHelper<Quiz>(_connection);
        await quizOrm.InsertAsync(quiz, token, "quizzes");
        await _connection.CloseAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<User>(_connection);
        var user = await orm.FindAsync<User>(new List<(string column, object value)> {("email", email)}, token);
        await _connection.CloseAsync();

        return user;
    }
    
    public async Task<ReturnUser> GetUserByIdAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<User>(_connection);
        var user = await orm.FindAsync<ReturnUser>(new List<(string column, object value)> {("id", id)}, token);
        await _connection.CloseAsync();

        return user;
    }

    public async Task<bool> CheckUserExists(User user, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<User>(_connection);
        var result = await orm.ExistsAsync(user, token);
        await _connection.CloseAsync();
        return result;
    }
}