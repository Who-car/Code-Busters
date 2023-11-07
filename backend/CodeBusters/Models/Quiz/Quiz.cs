using CodeBusters.Utils;

namespace CodeBusters.Models;

[QuizValidation]
public class Quiz
{
    public Guid Id { get; set; }
    public string? Topic { get; set; }
    public string[]? Tags { get; set; }
    public List<Question>? Questions { get; set; }
}