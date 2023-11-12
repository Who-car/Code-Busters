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

    public async Task CreateTableAsync<T>(string tableName, Column[] columns)
    {
        await _connection.OpenAsync();
        var orm = new OrmHelper<T>(_connection);
        await orm.CreateTable(tableName, columns);
        await _connection.CloseAsync();
    }

    public async Task AddNewUserAsync(User user)
    {
        await _connection.OpenAsync();
        var orm = new OrmHelper<User>(_connection);
        await orm.InsertAsync(user);
        await _connection.CloseAsync();
    }
    
    public async Task AddNewQuizAsync(Quiz quiz)
    {
        await _connection.OpenAsync();
        var quizOrm = new OrmHelper<Quiz>(_connection);
        await quizOrm.InsertAsync(quiz, "quizzes");
        await _connection.CloseAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        await _connection.OpenAsync();
        var orm = new OrmHelper<User>(_connection);
        var user = await orm.SearchWithParamsAsync<User>("email", email);
        await _connection.CloseAsync();

        return user;
    }
    
    public async Task<ReturnUser> GetUserByIdAsync(Guid id)
    {
        await _connection.OpenAsync();
        var orm = new OrmHelper<User>(_connection);
        var user = await orm.SearchWithParamsAsync<ReturnUser>("id", id);
        await _connection.CloseAsync();

        return user;
    }

    public async Task<bool> CheckUserExists(User user)
    {
        await _connection.OpenAsync();
        var orm = new OrmHelper<User>(_connection);
        var result = orm.Exists(user, "email");
        await _connection.CloseAsync();
        return result;
    }
}