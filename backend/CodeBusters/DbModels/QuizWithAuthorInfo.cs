using CodeBusters.DbModels;

namespace CodeBusters.Models;

public class QuizWithAuthorInfo
{
    public string Name { get; set; }
    public string Email { get; set; }
    public Quiz Quiz { get; set; }
}