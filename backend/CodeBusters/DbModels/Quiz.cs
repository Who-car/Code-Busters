using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CodeBusters.DbModels;

public class Quiz
{
    public Guid Id { get; set; }
    public string Author { get; set; }
    public string? Topic { get; set; }
    public Difficulty Difficulty { get; set; }
    public string Description { get; set; }
    public JsonArray Questions { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Difficulty
{
    [JsonPropertyName("easy")]
    Easy,
    [JsonPropertyName("medium")]
    Medium,
    [JsonPropertyName("hard")]
    Hard
}