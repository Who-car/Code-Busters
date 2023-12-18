using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using MyAspHelper.Attributes;

namespace MyAspHelper.Abstract;

public class HttpContextResult
{
    private readonly HttpListenerContext _context;
    internal MethodInfo? TargetMethod { get; set; }
    internal object[]? Parameters { get; set; }
    internal object? Controller { get; set; }
    public string? AuthToken => _context.Request.Cookies["token"]?.Value;
    public HttpListenerRequest Request => _context.Request;
    public HttpListenerResponse Response => _context.Response;
    
    public HttpContextResult(HttpListenerContext context)
    {
        _context = context;
    }

    public async Task SendResponse(int? code, string? info)
    {
        if (code != null) _context.Response.StatusCode = (int)code;
        if (info != null) await _context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(info)));
        if (info is not null) _context.Response.StatusDescription = info;
        _context.Response.OutputStream.Close();
    }
}