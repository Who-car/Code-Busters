namespace CodeBusters.Models;

public class QuizDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string? Topic { get; set; }
    public Difficulty Difficulty { get; set; }
}