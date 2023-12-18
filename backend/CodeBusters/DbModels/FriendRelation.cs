namespace CodeBusters.Models;

public class FriendRelation
{
    public Guid Id { get; set; }
    public Guid UserA { get; set; }
    public Guid UserB { get; set; }
    public Status Status { get; set; }
}