using System.Net;

namespace CodeBusters.Utils;

public static class WebHelper
{
    public static async Task SendResponseAsync(this HttpListenerContext context, int statusCode, string contentType, ReadOnlyMemory<byte> file)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = contentType;
        await context.Response.OutputStream.WriteAsync(file);
    }
}