using System.Net;
using System.Text;

namespace MyAspHelper.Abstract;

public class Controller
{
    public HttpListenerRequest Request { get; set; }
    public HttpListenerResponse Response { get; set; }

    protected async Task<bool> Ok(string? text = null)
    {
        try
        {
            Response.StatusCode = 200;
            if (text is not null)
                await Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(text));
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
        finally
        {
            Response.OutputStream.Close();
        }
    }

    protected async Task<bool> BadRequest(string? text = null)
    {
        try
        {
            Response.StatusCode = 400;
            if (text is not null)
                await Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(text));
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
        finally
        {
            Response.OutputStream.Close();
        }
    }
    
    protected async Task NotFound(string? text = null)
    {
        Response.StatusCode = 404;
        if (text is not null) 
            await Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(text));
        Response.OutputStream.Close();
    }
    
    protected async Task Unauthorized(string? text = null)
    {
        Response.StatusCode = 401;
        if (text is not null) 
            await Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(text));
        Response.OutputStream.Close();
    }
    
    protected async Task<bool> AccessDenied(string? text = null)
    {
        try
        {
            Response.StatusCode = 403;
            if (text is not null)
                await Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(text));
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
        finally
        {
            Response.OutputStream.Close();
        }
    }
}