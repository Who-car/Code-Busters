using System.Net;
using System.Text;
using System.Text.Json;

namespace MyAspHelper.Abstract;

public class Controller
{
    public HttpContextResult ContextResult { get; set; }

    protected ActionResult Ok(string? text = null, object? data = null)
    {
        //TODO: Адекватно ли тут использовать Result?
        //Вызывающий и вызываемый метод - асинхронные, а этот - синхронный
        text ??= "";
        return SendResponse(200, text, data).Result;
    }
    
    protected ActionResult Ok(byte[] data, string contentType)
    {
        return SendResponse(200, contentType, data).Result;
    }

    protected ActionResult BadRequest(string? text = null, object? data = null)
    {
        text ??= "";
        return SendResponse(400, text, data).Result;
    }
    
    protected ActionResult NotFound(string? text = null, object? data = null)
    {
        text ??= "";
        return SendResponse(404, text, data).Result;
    }
    
    protected ActionResult Unauthorized(string? text = null, object? data = null)
    {
        text ??= "";
        return SendResponse(401, text, data).Result;
    }
    
    protected ActionResult AccessDenied(string? text = null, object? data = null)
    {
        text ??= "";
        return SendResponse(403, text, data).Result;
    }

    private async Task<ActionResult> SendResponse(int statusCode, string statusDescription, object? data)
    {
        // Из официальной документации Microsoft:
        // В инструкции finally блок выполняется, когда элемент управления покидает try блок.
        // Элемент управления может покинуть try блок в результате:
        //
        // нормальное выполнение,
        // выполнение оператора перехода (т. е. return, break, continue или goto) или
        // распространение исключения из try блока.
        // https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/statements/exception-handling-statements#the-try-finally-statement
        try
        {
            ContextResult.Response.StatusCode = statusCode;
            ContextResult.Response.StatusDescription = statusDescription;
            if (data is not null)
                await ContextResult.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase})));
            return new ActionResult(true);
        }
        catch (Exception e)
        {
            return new ActionResult(false, e.Message);
        }
        finally
        {
            ContextResult.Response.OutputStream.Close();
        }
    }
    
    private async Task<ActionResult> SendResponse(int statusCode, string contentType, byte[] data)
    {
        try
        {
            ContextResult.Response.StatusCode = statusCode;
            ContextResult.Response.ContentEncoding = Encoding.UTF8;
            ContextResult.Response.ContentType = contentType;
            await ContextResult.Response.OutputStream.WriteAsync(data);
            return new ActionResult(true);
        }
        catch (Exception e)
        {
            return new ActionResult(false, e.Message);
        }
        finally
        {
            ContextResult.Response.OutputStream.Close();
        }
    }
}