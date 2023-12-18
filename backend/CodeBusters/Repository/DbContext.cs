using System.Linq.Expressions;
using System.Runtime.InteropServices.JavaScript;
using CodeBusters.DbModels;
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

    // TODO: по-хорошему first делать не надо
    public async Task<User> GetUserByEmailAsync(string email, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        var user = await Orm.FindAsync<User>(token,"users", new List<(string column, object value)> {("email", email)});
        await _connection.CloseAsync();

        return user.First();
    }
    
    public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);

        var user = await Orm.FindAsync<UserDto>(token, "users", new List<(string column, object value)> {("id", id)});
        
        await _connection.CloseAsync();

        return user.First();
    }
    
    public async Task<List<QuizDto>> GetQuizzesAsync(Guid cursor, int? count, CancellationToken token)
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
    
        return quiz.First();
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
    
    public async Task<List<Friend>> GetFriendsAsync(Guid id, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        var friends = await Orm.LeftJoinAsync(
            token,
            "friends",
            "users",
            (FriendRelation relation, User user) => user.Id != id,
            (relation, user) => new Friend { Id = user.Id, Name = user.Name },
            friendRelation => friendRelation.UserA == id || friendRelation.UserB == id);
        await _connection.CloseAsync();
    
        return friends;
    }
    
    public async Task AddFriendAsync(Guid id, Guid friendId, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        Orm.ConfigureConnection(_connection);
        try
        {
            List<FriendRelation> relation;
            relation = await Orm.FindAsync<FriendRelation>(token, "friends",
                new List<(string column, object value)> { ("user_b", id) });
            relation = await Orm.FindAsync<FriendRelation>(token, "friends",
                new List<(string column, object value)> { ("user_a", id) });
            var upd = relation.First().Status = Status.Accepted;
            await Orm.UpdateAsync(token, "friends", upd);
        }
        catch (KeyNotFoundException e)
        {
            var relation = new FriendRelation()
            {
                Id = Guid.NewGuid(),
                UserA = id,
                UserB = friendId,
                Status = Status.InvitationSent
            };
            await Orm.InsertAsync(token, "friends", relation);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}