namespace CodeBusters.Models;

public class QuizDto
{
    public string Name { get; set; }
    public Guid AuthorId { get; set; }
    public string? Topic { get; set; }
    public List<string>? Tags { get; set; }
}