using System.Net;
using System.Text.Json;
using CodeBusters.Database;
using CodeBusters.Models;
using CodeBusters.Utils;
using MyAspHelper.Abstract;
using MyAspHelper.Attributes;

namespace CodeBusters.Controllers.UserController;

public class UserController : Controller
{
    [Route("/api/User/register")]
    public async Task<bool> RegisterAsync()
    {
        var cancellationToken = new CancellationToken();
        
        using var sr = new StreamReader(Request.InputStream);
        var userStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var user = JsonSerializer.Deserialize<User>(userStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (user is null)
            return await BadRequest("All fields must be filled");
        
        var validationResult = CustomValidator.Validate(user);
        if (validationResult is { isSuccess: false })
            return await BadRequest(string.Join(';', validationResult.results.Select(er => er.ErrorMessage)));
            
        user.Password = Hasher.Hash(user.Password);
        var dbContext = new DbContext();
        if (await dbContext.CheckUserExists(user, cancellationToken))
            return await BadRequest("User already exists");
            
        user.Id = Guid.NewGuid();
        await dbContext.AddNewUserAsync(user, cancellationToken);

        return await Ok("User added successfully");
    }
    
    [Route("/api/User/login")]
    public async Task<bool> LoginAsync()
    {
        var cancellationToken = new CancellationToken();
        
        using var sr = new StreamReader(Request.InputStream);
        var loginStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var login = JsonSerializer.Deserialize<LoginModel>(loginStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (login is null)
            return await BadRequest("Either login or password is empty");
        
        var dbContext = new DbContext();
        //TODO: избавиться от выброса exception'a
        var user = new User();
        try
        {
            user = await dbContext.GetUserByEmailAsync(login.Email, cancellationToken);
        }
        catch (KeyNotFoundException e)
        {
            return await BadRequest($"No user with such email. Additional info: {e.Message}");
        }
        
        if (!Hasher.Validate(user.Password, login.Password))
            return await BadRequest("Passwords don't match");

        var token = JwtHelper<User>.GenerateToken(user);
        return await Ok(token);
    }
    
    [Authorize]
    [Route("/api/User/{id:guid}/get")]
    public async Task<bool> GetUserInfoAsync(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var token = Request.Headers["authToken"]!;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId != id)
            return await AccessDenied("Invalid Token");

        var db = new DbContext();
        var user = await db.GetUserByIdAsync(userId, cancellationToken);

        return await Ok(JsonSerializer.Serialize(user));
    }
}