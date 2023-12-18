using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeBusters.DbModels;
using CodeBusters.Models;
using CodeBusters.Repository;
using CodeBusters.Utils;
using MyAspHelper;
using MyAspHelper.Abstract;
using MyAspHelper.Attributes;
using MyAspHelper.Attributes.HttpMethods;
using MyAspHelper.AuthSchemas;

namespace CodeBusters.Controllers.QuizController;

public class QuizController : Controller
{
    private static QuizContext QuizContext = new QuizContext();
    
    [HttpPost]
    [Authorize]
    [Route("/api/Quiz/post")]
    public async Task<ActionResult> AddQuizAsync()
    {
        var cancellationToken = new CancellationToken();
        var token = ContextResult.AuthToken;
        var userId = JwtHelper<User>.ValidateToken(token!);
        
        if (userId == Guid.Empty)
            return BadRequest("Invalid token",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"Token has expired. Please, log in to continue"}
                });
        
        using var sr = new StreamReader(ContextResult.Request.InputStream);
        var quizStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var quiz = JsonSerializer.Deserialize<Quiz>(quizStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        if (quiz is null)
            return BadRequest("Quiz insert failed",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"All fields must be filled"}
                });
        
        var validationResult = QuizValidator.TryValidate(quiz);
        if (!validationResult.IsValid)
            return BadRequest("Quiz insert failed",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = validationResult.Errors.Select(er => er.ErrorMessage).ToList()
                });
            
        var dbContext = new DbContext();
        quiz.Id = Guid.NewGuid();
        await dbContext.AddNewQuizAsync(quiz, cancellationToken);

        return Ok(
            "Quiz insert success",
            new SuccessResponseDto()
            {
               Result = true
            });
    }
    
    [HttpGet]
    [Route("/api/Quiz/get")]
    public async Task<ActionResult> GetAllQuizzes()
    {
        var cancellationToken = new CancellationToken();
        var db = new DbContext();
        var quizzes = await db.GetQuizzesAsync(QuizContext.LastSentQuizId, QuizContext.Count, cancellationToken);
        if (quizzes.Count > 0)
            QuizContext.LastSentQuizId = quizzes.Last().Id;
        return Ok("", quizzes);
    }
    
    [HttpGet]
    [Route("/api/Quiz/get/{count:int}")]
    public async Task<ActionResult> GetAllQuizzesFirstTime(int count)
    {
        var cancellationToken = new CancellationToken();
        var db = new DbContext();
        QuizContext = new QuizContext();
        var quizzes = await db.GetQuizzesAsync(QuizContext.LastSentQuizId, count, cancellationToken);
        if (quizzes.Count <= 0)
            return Empty();
        QuizContext.LastSentQuizId = quizzes.Last().Id;
        QuizContext.Count = count;
        return Ok("", quizzes);
    }
    
    [HttpGet]
    [Route("api/Quiz/{id:guid}/get")]
    public async Task<ActionResult> GetQuiz(Guid id)
    {
        var cancellationToken = new CancellationToken();
        
        var db = new DbContext();
        var quiz = await db.GetQuizAsync(id, cancellationToken);
        
        return Ok("", quiz);
    }
    
    [HttpGet]
    [Route("api/Quiz/{name:string}/avatar/get")]
    public async Task<ActionResult> GetQuizAvatar(string name)
    {
        var cancellationToken = new CancellationToken();
        try
        {
            var image = await File.ReadAllBytesAsync(Path.Combine(App.Settings["StaticResourcesPath"]!, "QuizAvatars", $"{name}.jpg"), cancellationToken);
            return Ok(image, "image/jpeg");
        }
        catch (FileNotFoundException e)
        {
            var def = await File.ReadAllBytesAsync(Path.Combine(App.Settings["StaticResourcesPath"]!, "QuizAvatars", "Default.png"));
            return Empty(def, "image/png");
        }
    }

    [HttpGet]
    [Route("api/Quiz/{id:guid}/comments/get")]
    public async Task<ActionResult> GetComments(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var db = new DbContext();
        var comments = await db.GetCommentsAsync(id, cancellationToken);
        return Ok("", comments);
    }
    
    [HttpPost]
    [Authorize]
    [Route("api/Quiz/{id:guid}/comments/add")]
    public async Task<ActionResult> AddComment(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var token = ContextResult.AuthToken;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId == Guid.Empty)
            return BadRequest("Invalid token",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"Token has expired. Please, log in to continue"}
                });
        
        using var sr = new StreamReader(ContextResult.Request.InputStream);
        var commentStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var comment = JsonSerializer.Deserialize<Comment>(commentStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.Strict
        });

        if (comment is null)
            return BadRequest("Invalid token",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"Can't add empty comment"}
                });
        
        var dbContext = new DbContext();
        comment.Id = Guid.NewGuid();
        comment.AuthorId = userId;
        comment.QuizId = id;
        await dbContext.AddCommentAsync(comment, cancellationToken);

        return Ok();
    }
    
    [HttpPost]
    [Authorize]
    [Route("api/Quiz/{name:string}")]
    public async Task<ActionResult> PostAvatar(string name)
    {
        return Multipart("QuizAvatars", name);
    }
}