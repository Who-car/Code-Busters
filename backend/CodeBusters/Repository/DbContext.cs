using System.Data;
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
                                            "Database=CodeBusters";
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
        var orm = new OrmHelper<Quiz>(_connection);
        await orm.InsertAsync(quiz);
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
    
    public async Task<User> GetUserByIdAsync(Guid id)
    {
        await _connection.OpenAsync();
        var orm = new OrmHelper<User>(_connection);
        var user = await orm.SearchWithParamsAsync<User>("id", id);
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