using System.Net;
using System.Text;
using System.Text.Json;

namespace MyAspHelper.Abstract;

public class Controller
{
    public HttpContextResult ContextResult { get; set; }

    protected ActionResult Ok(string? text = null, object? data = null)
    {
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

    protected ActionResult Empty(byte[]? data = null, string? contentType = null)
    {
        if (data is null) return SendResponse(207, "No content", null).Result;
        return SendResponse(207, contentType, data).Result;
    }

    protected ActionResult Multipart(string path, string name)
    {
        SaveFile(ContextResult.Request.ContentEncoding, 
            GetBoundary(ContextResult.Request.ContentType), 
            ContextResult.Request.InputStream, 
            Path.Combine(App.Settings["StaticResourcesPath"]!, $"{path}", $"{name}.jpg"));
        return SendResponse(201, "", null).Result;
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

    private static String GetBoundary(String ctype) 
    { 
        return "--" + ctype.Split(';')[1].Split('=')[1]; 
    }

    private static void SaveFile(Encoding enc, String boundary, Stream input, string path)
    {
        var boundaryBytes = enc.GetBytes(boundary);
        var boundaryLen = boundaryBytes.Length;

        using FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write);
        var buffer = new Byte[1024];
        var len = input.Read(buffer, 0, 1024);
        var startPos = -1;

        // Find start boundary
        while (true)
        {
            if (len == 0)
            {
                throw new Exception("Start Boundaray Not Found");
            }

            startPos = IndexOf(buffer, len, boundaryBytes);
            if (startPos >= 0)
            {
                break;
            }
            else
            {
                Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                len = input.Read(buffer, boundaryLen, 1024 - boundaryLen);
            }
        }

        // Skip four lines (Boundary, Content-Disposition, Content-Type, and a blank)
        for (Int32 i = 0; i < 4; i++)
        {
            while (true)
            {
                if (len == 0)
                {
                    throw new Exception("Preamble not Found.");
                }

                startPos = Array.IndexOf(buffer, enc.GetBytes("\n")[0], startPos);
                if (startPos >= 0)
                {
                    startPos++;
                    break;
                }
                else
                {
                    len = input.Read(buffer, 0, 1024);
                }
            }
        }

        Array.Copy(buffer, startPos, buffer, 0, len - startPos);
        len = len - startPos;

        while (true)
        {
            Int32 endPos = IndexOf(buffer, len, boundaryBytes);
            if (endPos >= 0)
            {
                if (endPos > 0) output.Write(buffer, 0, endPos-2);
                break;
            }
            else if (len <= boundaryLen)
            {
                throw new Exception("End Boundaray Not Found");
            }
            else
            {
                output.Write(buffer, 0, len - boundaryLen);
                Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                len = input.Read(buffer, boundaryLen, 1024 - boundaryLen) + boundaryLen;
            }
        }
    }

    private static Int32 IndexOf(Byte[] buffer, Int32 len, Byte[] boundaryBytes)
    {
        for (Int32 i = 0; i <= len - boundaryBytes.Length; i++)
        {
            Boolean match = true;
            for (Int32 j = 0; j < boundaryBytes.Length && match; j++)
            {
                match = buffer[i + j] == boundaryBytes[j];
            }

            if (match)
            {
                return i;
            }
        }

        return -1;
    }
}