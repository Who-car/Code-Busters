using CodeBusters.Models;
using MyAspHelper.Abstract;
using MyOrmHelper;
using Npgsql;

namespace CodeBusters.Repository;

public class DbContext : IRepository
{
    private static string _connectionString;
    private readonly NpgsqlConnection _connection = new(_connectionString);

    public static void ConfigureDb(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CreateTableAsync<T>(string tableName, CancellationToken token, Column[] columns)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<T>(_connection);
        await orm.CreateTableAsync(tableName, token, columns);
        await _connection.CloseAsync();
    }

    public async Task CreateEnumAsync(Type enumType, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<Difficulty>(_connection);
        await orm.CreateEnumAsync(enumType, token);
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
    
    public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<User>(_connection);
        var user = await orm.FindAsync<UserDto>(new List<(string column, object value)> {("id", id)}, token);
        await _connection.CloseAsync();

        return user;
    }
    
    public async Task<List<Quiz>> GetQuizzesAsync(Guid cursor, int count, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<Quiz>(_connection);
        var quizzes = await orm.SelectAsync<Quiz>(cursor.ToString(), count, token, "quizzes");
        await _connection.CloseAsync();
    
        return quizzes;
    }
    
    public async Task<Quiz> GetQuizAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<Quiz>(_connection);
        var quiz = await orm.FindAsync<Quiz>(new List<(string column, object value)> {("id", id)}, token, "quizzes");
        await _connection.CloseAsync();
    
        return quiz;
    }
    
    public async Task<List<Comment>> GetCommentsAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<Comment>(_connection);
        var comments = await orm.SelectAsync<Comment>("", null, token);
        await _connection.CloseAsync();
    
        return comments;
    }
    
    public async Task AddCommentAsync(Comment comment, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<Comment>(_connection);
        await orm.InsertAsync(comment, token);
        await _connection.CloseAsync();
    }

    public async Task<bool> CheckUserExists(User user, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        var orm = new OrmHelper<User>(_connection);
        try
        {
            await orm.FindAsync<User>(new List<(string column, object value)> { ("email", user.Email) }, token);
            return true;
        }
        catch (KeyNotFoundException e)
        {
            return false;
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}