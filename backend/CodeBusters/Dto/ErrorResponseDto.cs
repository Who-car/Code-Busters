namespace CodeBusters.Models;

public class ErrorResponseDto
{
    public bool Result { get; set; }
    public List<string> Errors { get; set; }
}