using System.Net;
using System.Text.Json;
using CodeBusters.Database;
using CodeBusters.Models;
using CodeBusters.Utils;
using MyAspHelper.Abstract;
using MyAspHelper.Attributes;

namespace CodeBusters.Controllers.QuizController;

public class QuizController : Controller
{
    [Authorize]
    [Route("/api/Quiz/add")]
    public async Task<bool> AddQuizAsync()
    {
        var cancellationToken = new CancellationToken();
        var token = Request.Headers["authToken"]!;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId == Guid.Empty)
            return await BadRequest("Invalid token");
        
        using var sr = new StreamReader(Request.InputStream);
        var quizStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var quiz = JsonSerializer.Deserialize<Quiz>(quizStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (quiz is null)
            return await BadRequest("All fields must be filled");
        
        var validationResult = CustomValidator.Validate(quiz);
        if (validationResult is { isSuccess: false })
            return await BadRequest(string.Join(';', validationResult.results.Select(er => er.ErrorMessage)));
            
        var dbContext = new DbContext();
        quiz.Id = Guid.NewGuid();
        quiz.AuthorId = userId;
        await dbContext.AddNewQuizAsync(quiz, cancellationToken);

        return await Ok("Quiz added successfully");
    }
    
    [Route("/api/Quiz/get/{count:int}")]
    public async Task<bool> GetAllQuizzes(int count)
    {
        var cancellationToken = new CancellationToken();
        var db = new DbContext();
        // var quizzes = await db.GetQuizzesAsync(count, cancellationToken);
        var quizzes = new List<Quiz>();
        return await Ok(JsonSerializer.Serialize(quizzes));
    }

    [Route("api/Quiz/get/{id:guid}")]
    public async Task<bool> GetQuiz(Guid id)
    {
        var cancellationToken = new CancellationToken();
        
        var db = new DbContext();
        // var quiz = await db.GetQuizAsync(id, cancellationToken);
        var quiz = new Quiz();
        return await Ok(JsonSerializer.Serialize(quiz));
    }

    [Route("api/Quiz/{id:guid}/comments/get")]
    public async Task<bool> GetComments(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var db = new DbContext();
        // var comments = await db.GetCommentsAsync(id, cancellationToken);
        var comments = new List<Comment>();
        return await Ok(JsonSerializer.Serialize(comments));
    }
    
    [Authorize]
    [Route("api/Quiz/{id:guid}/comments/add")]
    public async Task<bool> AddComment(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var token = Request.Headers["authToken"]!;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId == Guid.Empty)
            return await BadRequest("Invalid token");
        
        using var sr = new StreamReader(Request.InputStream);
        var commentStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var comment = JsonSerializer.Deserialize<Comment>(commentStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (comment is null)
            return await BadRequest("Can't add empty comment");
        
        var dbContext = new DbContext();
        comment.Id = Guid.NewGuid();
        comment.AuthorId = userId;
        comment.QuizId = id;
        // await dbContext.AddNewCommentAsync(comment, cancellationToken);

        return await Ok();
    }
}