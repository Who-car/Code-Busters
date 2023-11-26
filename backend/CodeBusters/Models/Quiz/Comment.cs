namespace CodeBusters.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid QuizId { get; set; }
    public double Rating { get; set; }
    public string Text { get; set; }
}