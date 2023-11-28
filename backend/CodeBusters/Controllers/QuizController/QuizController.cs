using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeBusters.Models;
using CodeBusters.Repository;
using CodeBusters.Utils;
using MyAspHelper.Abstract;
using MyAspHelper.Attributes;
using MyAspHelper.Attributes.HttpMethods;

namespace CodeBusters.Controllers.QuizController;

public class QuizController : Controller
{
    private static readonly QuizContext _context = new QuizContext();
    
    [HttpPost]
    [Authorize]
    [Route("/api/Quiz/post")]
    public async Task<ActionResult> AddQuizAsync()
    {
        var cancellationToken = new CancellationToken();
        var token = Request.Headers["authToken"]!;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId == Guid.Empty)
            return BadRequest("Invalid token");
        
        using var sr = new StreamReader(Request.InputStream);
        var quizStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var quiz = JsonSerializer.Deserialize<Quiz>(quizStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            
        });

        if (quiz is null)
            return BadRequest("All fields must be filled");
        
        var validationResult = QuizValidator.TryValidate(quiz);
        if (!validationResult.IsValid)
            return BadRequest(string.Join(';', validationResult.Errors.Select(er => er.ErrorMessage)));
            
        var dbContext = new DbContext();
        quiz.Id = Guid.NewGuid();
        quiz.AuthorId = userId;
        await dbContext.AddNewQuizAsync(quiz, cancellationToken);

        return Ok("Quiz added successfully");
    }
    
    [HttpGet]
    [Route("/api/Quiz/get/{count:int}")]
    public async Task<ActionResult> GetAllQuizzes(int count)
    {
        var cancellationToken = new CancellationToken();
        var db = new DbContext();
        var quizzes = await db.GetQuizzesAsync(_context.LastSentQuizId, count, cancellationToken);
        if (quizzes.Count > 0)
            _context.LastSentQuizId = quizzes.Last().Id;
        return Ok(JsonSerializer.Serialize(quizzes));
    }
    
    [HttpGet]
    [Route("api/Quiz/{id:guid}/get")]
    public async Task<ActionResult> GetQuiz(Guid id)
    {
        var cancellationToken = new CancellationToken();
        
        var db = new DbContext();
        var quiz = await db.GetQuizAsync(id, cancellationToken);
        return Ok(JsonSerializer.Serialize(quiz));
    }

    [HttpGet]
    [Route("api/Quiz/{id:guid}/comments/get")]
    public async Task<ActionResult> GetComments(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var db = new DbContext();
        var comments = await db.GetCommentsAsync(id, cancellationToken);
        return Ok(JsonSerializer.Serialize(comments));
    }
    
    [HttpPost]
    [Authorize]
    [Route("api/Quiz/{id:guid}/comments/add")]
    public async Task<ActionResult> AddComment(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var token = Request.Headers["authToken"]!;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId == Guid.Empty)
            return BadRequest("Invalid token");
        
        using var sr = new StreamReader(Request.InputStream);
        var commentStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var comment = JsonSerializer.Deserialize<Comment>(commentStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.Strict
        });

        if (comment is null)
            return BadRequest("Can't add empty comment");
        
        var dbContext = new DbContext();
        comment.Id = Guid.NewGuid();
        comment.AuthorId = userId;
        comment.QuizId = id;
        await dbContext.AddCommentAsync(comment, cancellationToken);

        return Ok();
    }
}