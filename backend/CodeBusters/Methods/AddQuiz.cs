﻿using System.Net;
using System.Text;
using System.Text.Json;
using CodeBusters.Database;
using CodeBusters.Models;
using CodeBusters.Utils;
using MyAspHelper.Attributes;

namespace CodeBusters.Methods;

public static partial class Methods
{
    [Route("/addQuiz")]
    [Authorize]
    public static async Task AddQuizAsync(HttpListenerRequest request, HttpListenerResponse response)
    {
        using var sr = new StreamReader(request.InputStream);
        var quizStr = await sr.ReadToEndAsync().ConfigureAwait(false);
        var body = new Response();
        
        var quiz = JsonSerializer.Deserialize<Quiz>(quizStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (quiz != null)
        {
            var validationResult = CustomValidator.Validate(quiz);
            
            if (validationResult is { isSuccess: false })
            {
                body.Success = false;
                body.ErrorInfo = validationResult.results.Select(er => er.ErrorMessage).ToList();
                response.ContentType = "application/json";
                response.StatusCode = 400;
                await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
                response.OutputStream.Close();
                return;
            }
            
            var dbContext = new DbContext();
            
            quiz.Id = Guid.NewGuid();
            await dbContext.AddNewQuizAsync(quiz);
        
            body.Success = true;
            body.Text = "added quiz successfully";
            response.ContentType = "application/json; charset=utf-8";
            response.StatusCode = 200;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
            response.OutputStream.Close();
        }
    }
}