using CodeBusters.Utils;

namespace CodeBusters.Models;

[UserValidation]
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Quiz> Quizzes { get; set; }
}