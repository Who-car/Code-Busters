using System.ComponentModel.DataAnnotations.Schema;
using CodeBusters.Utils;

namespace CodeBusters.Models;

[QuizValidation]
public class Quiz
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string? Topic { get; set; }
    public List<string>? Tags { get; set; }
    [Column(TypeName = "jsonb")]
    public List<Question>? Questions { get; set; }
}