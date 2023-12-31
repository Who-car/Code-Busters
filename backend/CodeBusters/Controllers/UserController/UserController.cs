﻿using System.Net;
using System.Text;
using System.Text.Json;
using CodeBusters.Models;
using CodeBusters.Repository;
using CodeBusters.Utils;
using MyAspHelper;
using MyAspHelper.Abstract;
using MyAspHelper.Attributes;
using MyAspHelper.Attributes.HttpMethods;
using MyAspHelper.AuthSchemas;

namespace CodeBusters.Controllers.UserController;

public class UserController : Controller
{
    [HttpPost]
    [Route("/api/User/register")]
    public async Task<ActionResult> RegisterAsync()
    {
        var cancellationToken = new CancellationToken();
        
        using var sr = new StreamReader(ContextResult.Request.InputStream);
        var userStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var user = JsonSerializer.Deserialize<User>(userStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (user is null)
            return BadRequest(
                "Authorization failed", 
                new ErrorResponseDto
                {
                    Result = false, 
                    Errors = new List<string> { "All fields must be filled" }
                });
        
        var validationResult = UserValidator.TryValidate(user);
        if (!validationResult.IsValid)
            return BadRequest(
                "Authorization failed", 
                new ErrorResponseDto
                {
                    Result = false, 
                    Errors = validationResult.Errors.Select(er => er.ErrorMessage).ToList()
                });
            
        user.Password = Hasher.Hash(user.Password);
        var dbContext = new DbContext();
        if (await dbContext.CheckUserExists(user, cancellationToken))
            return BadRequest(
                "Authorization failed", 
                new ErrorResponseDto
                {
                    Result = false, 
                    Errors = new List<string> { "User already exists" }
                });
            
        user.Id = Guid.NewGuid();
        await dbContext.AddNewUserAsync(user, cancellationToken);
        var token = JwtHelper<User>.GenerateToken(user);

        return Ok(
            "Successfully authorized", 
            new SuccessResponseDto
            {
                Result = true, 
                Token = token,
                Id = user.Id
            });
    }
    
    [HttpPost]
    [Route("/api/User/login")]
    public async Task<ActionResult> LoginAsync()
    {
        var cancellationToken = new CancellationToken();
        
        using var sr = new StreamReader(ContextResult.Request.InputStream);
        var loginStr = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var login = JsonSerializer.Deserialize<LoginDto>(loginStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (login is null)
            return BadRequest(
                "Authorization failed",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"Either login or password is empty"}
                });
        
        var dbContext = new DbContext();
        User user;
        try
        {
            user = await dbContext.GetUserByEmailAsync(login.Email, cancellationToken);
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(
                "Authorization failed",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {$"No user with such email. Additional info: {e.Message}"}
                });
        }
        
        if (!Hasher.Validate(user.Password, login.Password)) 
            return BadRequest(
                "Authorization failed",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"Passwords don't match"}
                });

        var token = JwtHelper<User>.GenerateToken(user);
        return Ok(
            "Successfully authorized", 
            new SuccessResponseDto
            {
                Result = true, 
                Token = token,
                Id = user.Id
            });
    }
    
    [HttpGet]
    [Authorize]
    [Route("/api/User/{id:guid}/get")]
    public async Task<ActionResult> GetUserInfoAsync(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var token = ContextResult.AuthToken;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId != id)
            return AccessDenied(
                "Invalid Token",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"Token has expired. Please, log in to continue"}
                });

        var db = new DbContext();
        try
        {
            var user = await db.GetUserByIdAsync(userId, cancellationToken);
            return Ok("", user);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound("User not found",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"No user with such id"}
                });
        }
    }
    
    [HttpPost]
    [Authorize]
    [Route("api/User/{id:guid}/avatar/post")]
    public async Task<ActionResult> PostAvatar(Guid id)
    {
        return Multipart("UserAvatars", $"{id}");
    }

    [HttpGet]
    [Authorize]
    [Route("api/User/{id:guid}/avatar/get")]
    public async Task<ActionResult> GetAvatar(Guid id)
    {
        var cancellationToken = new CancellationToken();
        try
        {
            var image = await File.ReadAllBytesAsync(Path.Combine(App.Settings["StaticResourcesPath"]!, "UserAvatars", $"{id}.jpg"), cancellationToken);
            return Ok(image, "image/jpeg");
        }
        catch (FileNotFoundException e)
        {
            return Empty();
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("/api/User/{id:guid}/friends/get")]
    public async Task<ActionResult> GetFriends(Guid id)
    {
        var cancellationToken = new CancellationToken();
        var token = ContextResult.AuthToken;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId != id)
            return AccessDenied(
                "Invalid Token",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"Token has expired. Please, log in to continue"}
                });

        var db = new DbContext();
        try
        {
            var user = await db.GetFriendsAsync(userId, cancellationToken);
            return Ok("", user);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound("User not found",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"No user with such id"}
                });
        }
    }
    
    [HttpPost]
    [Authorize]
    [Route("/api/User/{id:guid}/friends/add/{friendId:guid}")]
    public async Task<ActionResult> AddFriend(Guid id, Guid friendId)
    {
        var cancellationToken = new CancellationToken();
        var token = ContextResult.AuthToken;
        var userId = JwtHelper<User>.ValidateToken(token);

        if (userId != id)
            return AccessDenied(
                "Invalid Token",
                new ErrorResponseDto()
                {
                    Result = false,
                    Errors = new List<string>() {"Token has expired. Please, log in to continue"}
                });

        var db = new DbContext();
        await db.AddFriendAsync(userId, friendId, cancellationToken);
        return Ok("Invitation sent");
    }
}