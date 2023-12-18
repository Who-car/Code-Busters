using CodeBusters.DbModels;

namespace CodeBusters.Models;

public class QuizDto
{
    public Guid Id { get; set; }
    public string Author { get; set; }
    public string? Topic { get; set; }
    public string? Description { get; set; }
    public Difficulty Difficulty { get; set; }
}