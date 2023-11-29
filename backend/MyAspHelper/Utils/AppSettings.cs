using System.Text.Json.Nodes;

namespace MyAspHelper.Utils;

public class AppSettings
{
    private readonly JsonNode _json;

    public AppSettings()
    {
        using var jsonReader = new StreamReader("appsettings.json");
        var json = jsonReader.ReadToEnd();
        _json = JsonNode.Parse(json)!;
    }

    public string? this[string key] => _json[key]?.ToString();
}