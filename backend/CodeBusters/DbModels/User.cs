using CodeBusters.Utils;

namespace CodeBusters.Models;

public class User : IAuthorizable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}