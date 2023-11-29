using System.Runtime.InteropServices.JavaScript;
using CodeBusters.Models;
using MyAspHelper.Abstract;
using MyOrmHelper;
using Npgsql;

namespace CodeBusters.Repository;

public class DbContext : IRepository
{
    private static string _connectionString;
    private static readonly OrmHelper Orm = new();
    private readonly NpgsqlConnection _connection = new(_connectionString);

    public static void ConfigureDb(string connectionString)
    {
        _connectionString = connectionString;
    }

    //Сделать дженериковую перегрузку
    public async Task CreateTableAsync(string tableName, CancellationToken token, Column[] columns)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        await Orm.CreateTableAsync(tableName, token, columns);
        await _connection.CloseAsync();
    }

    public async Task CreateEnumAsync(Type enumType, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        await Orm.CreateTypeAsync(enumType, "enum", token);
        await _connection.CloseAsync();
    }

    public async Task AddNewUserAsync(User user, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        await Orm.InsertAsync(token, "users", user);
        await _connection.CloseAsync();
    }
    
    public async Task AddNewQuizAsync(Quiz quiz, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        await Orm.InsertAsync(token, "quizzes", quiz);
        await _connection.CloseAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        var user = await Orm.FindAsync<User>(token,"users", new List<(string column, object value)> {("email", email)});
        await _connection.CloseAsync();

        return user;
    }
    
    public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        var userQuizzes = await Orm.LeftJoinAsync(
            token, 
            "users", 
            "quizzes", 
            (user, quiz) => user.Id == quiz.AuthorId,
            (User user, Quiz quiz) => new QuizWithAuthorInfo() {Name = user.Name, Email = user.Email, Quiz = quiz},
            user => user.Id == id);
        await _connection.CloseAsync();

        if (userQuizzes.Count == 0)
            throw new KeyNotFoundException($"User with id {id} doesn't have quizzes");

        var user = new UserDto()
        {
            Name = userQuizzes.FirstOrDefault()!.Name,
            Email = userQuizzes.FirstOrDefault()!.Email,
            Quizzes = userQuizzes.Select(q => q.Quiz).ToList()
        };

        return user;
    }
    
    public async Task<List<QuizDto>> GetQuizzesAsync(Guid cursor, int count, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        var quizzes = await Orm.SelectAsync<QuizDto>(token, "quizzes", cursor.ToString(), count);
        await _connection.CloseAsync();
    
        return quizzes;
    }
    
    public async Task<Quiz> GetQuizAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        var quiz = await Orm.FindAsync<Quiz>(token, "quizzes", new List<(string column, object value)> {("id", id)});
        await _connection.CloseAsync();
    
        return quiz;
    }
    
    public async Task<List<Comment>> GetCommentsAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        var comments = await Orm.SelectAsync<Comment>(token, "comments");
        await _connection.CloseAsync();
    
        return comments;
    }
    
    public async Task AddCommentAsync(Comment comment, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        await Orm.InsertAsync(token, "comments", comment);
        await _connection.CloseAsync();
    }

    public async Task<bool> CheckUserExists(User user, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        try
        {
            await Orm.FindAsync<User>(token, "users", new List<(string column, object value)> { ("email", user.Email) });
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