namespace CodeBusters.Models;

public class Comment
{
    public User Author { get; set; }
    public Quiz Quiz { get; set; }
    public double Rating { get; set; }
    public string Text { get; set; }
}