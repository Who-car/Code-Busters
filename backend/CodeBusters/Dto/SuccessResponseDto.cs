namespace CodeBusters.Models;

public class SuccessResponseDto
{
    public bool Result { get; set; }
    public string Token { get; set; }
    public Guid Id { get; set; }
}