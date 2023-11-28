using System.Net;
using System.Text;

namespace MyAspHelper.Abstract;

public class Controller
{
    public HttpListenerRequest Request { get; set; }
    public HttpListenerResponse Response { get; set; }

    protected ActionResult Ok(string? text = null)
    {
        //TODO: Адекватно ли тут использовать Result?
        //Вызывающий и вызываемый метод - асинхронные, а этот - синхронный
        return SendResponse(200, text).Result;
    }
    
    protected ActionResult Ok(byte[] data, string contentType)
    {
        return SendResponse(200, contentType, data).Result;
    }

    protected ActionResult BadRequest(string? text = null)
    {
        return SendResponse(400, text).Result;
    }
    
    protected ActionResult NotFound(string? text = null)
    {
        return SendResponse(404, text).Result;
    }
    
    protected ActionResult Unauthorized(string? text = null)
    {
        return SendResponse(401, text).Result;
    }
    
    protected ActionResult AccessDenied(string? text = null)
    {
        return SendResponse(403, text).Result;
    }

    private async Task<ActionResult> SendResponse(int statusCode, string? text)
    {
        // Из официальной документации Microsoft:
        // В инструкции finally блок выполняется, когда элемент управления покидает try блок.
        // Элемент управления может покинуть try блок в результате:
        //
        // нормальное выполнение,
        // выполнение оператора перехода (т. е. return, break, continueили goto) или
        // распространение исключения из try блока.
        // https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/statements/exception-handling-statements#the-try-finally-statement
        try
        {
            Response.StatusCode = statusCode;
            if (text is not null)
                await Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(text));
            return new ActionResult(true);
        }
        catch (Exception e)
        {
            return new ActionResult(false, e.Message);
        }
        finally
        {
            Response.OutputStream.Close();
        }
    }
    
    private async Task<ActionResult> SendResponse(int statusCode, string contentType, byte[] data)
    {
        try
        {
            Response.StatusCode = statusCode;
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contentType;
            await Response.OutputStream.WriteAsync(data);
            return new ActionResult(true);
        }
        catch (Exception e)
        {
            return new ActionResult(false, e.Message);
        }
        finally
        {
            Response.OutputStream.Close();
        }
    }
}