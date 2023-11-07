namespace CodeBusters.Models;

public class ReturnUser
{
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Quiz> Quizzes { get; set; }
}