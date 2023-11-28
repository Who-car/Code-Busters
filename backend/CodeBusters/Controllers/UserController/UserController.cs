using System.Net;
using System.Text.Json;
using CodeBusters.Models;
using CodeBusters.Repository;
using CodeBusters.Utils;
using MyAspHelper.Abstract;
using MyAspHelper.Attributes;
using MyAspHelper.Attributes.HttpMethods;

namespace CodeBusters.Controllers.UserController;

public class UserController : Controller
{
    [HttpPost]
    [Route("/api/User/register")]
    public async Task<ActionResult> RegisterAsync()
    {
        var cancellationToken = new CancellationToken();
        
        using var sr = new StreamReader(Request.InputStream);
        var userStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var user = JsonSerializer.Deserialize<User>(userStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (user is null)
            return BadRequest("All fields must be filled");
        
        var validationResult = UserValidator.TryValidate(user);
        if (!validationResult.IsValid)
            return BadRequest(string.Join(';', validationResult.Errors.Select(er => er.ErrorMessage)));
            
        user.Password = Hasher.Hash(user.Password);
        var dbContext = new DbContext();
        if (await dbContext.CheckUserExists(user, cancellationToken))
            return BadRequest("User already exists");
            
        user.Id = Guid.NewGuid();
        await dbContext.AddNewUserAsync(user, cancellationToken);

        return Ok("User added successfully");
    }
    
    [HttpPost]
    [Route("/api/User/login")]
    public async Task<ActionResult> LoginAsync()
    {
        var cancellationToken = new CancellationToken();
        
        using var sr = new StreamReader(Request.InputStream);
        var loginStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var login = JsonSerializer.Deserialize<LoginDto>(loginStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (login is null)
            return BadRequest("Either login or password is empty");
        
        var dbContext = new DbContext();
        //TODO: избавиться от выброса exception'a
        var user = new User();
        try
        {
            user = await dbContext.GetUserByEmailAsync(login.Email, cancellationToken);
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest($"No user with such email. Additional info: {e.Message}");
        }
        
        if (!Hasher.Validate(user.Password, login.Password))
            return BadRequest("Passwords don't match");

        var token = JwtHelper<User>.GenerateToken(user);
        return Ok(token);
    }
    
    [HttpGet]
    [Authorize]
    [Route("/api/User/{id:guid}/get")]
    public async Task<ActionResult> GetUserInfoAsync(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var token = Request.Headers["authToken"]!;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId != id)
            return AccessDenied("Invalid Token");

        var db = new DbContext();
        var user = await db.GetUserByIdAsync(userId, cancellationToken);

        return Ok(JsonSerializer.Serialize(user));
    }

    //TODO: отлов возможных ошибок
    //TODO: починить путь (возможно относительно корневой папки системы?)
    [HttpPost]
    [Authorize]
    [Route("api/User/{id:guid}/avatar/post")]
    public async Task<ActionResult> PostAvatar(Guid id)
    {
        await using var body = Request.InputStream;
        var buffer = new byte[4096];
        var cancellationToken = new CancellationToken();
        using var ms = new MemoryStream();
        int bytesRead;
        while ((bytesRead = await body.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            ms.Write(buffer, 0, bytesRead);
        }

        var imageData = ms.ToArray();

        Directory.CreateDirectory("avatars");
        await File.WriteAllBytesAsync($"avatars/{id}.jpg", imageData, cancellationToken);

        return Ok("avatar added");
    }

    [HttpGet]
    [Authorize]
    [Route("api/User/{id:guid}/avatar/get")]
    public async Task<ActionResult> GetAvatar(Guid id)
    {
        var cancellationToken = new CancellationToken();
        byte[] image;
        try
        {
            image = await File.ReadAllBytesAsync($"avatars/{id}.jpg", cancellationToken);
        }
        catch (FileNotFoundException e)
        {
            return NotFound("User doesn't have avatar"); 
        }

        return Ok(image, "image/jpeg");
    }
}