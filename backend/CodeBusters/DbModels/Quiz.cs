using System.Text.Json.Nodes;

namespace CodeBusters.Models;

public class Quiz
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string? Topic { get; set; }
    public List<string>? Tags { get; set; }
    public JsonArray Questions { get; set; }
}