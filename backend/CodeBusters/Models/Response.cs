namespace CodeBusters.Models;

public class Response
{
    public bool Success { get; set; }
    public List<string?> ErrorInfo { get; set; } = new();
    public string? Token { get; set; }
    public string? Text { get; set; }
}