using CodeBusters.DbModels;

namespace CodeBusters.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Quiz> Quizzes { get; set; }
}