using System.Text.Json;

namespace MyAspHelper.Utils;

public class ConnectionString
{
    public string Host { get; init; }
    public string Port { get; init; }
    public string Username { get; init; }
    public string Password { get; init; }
    public string Database { get; init; }
    public const string DefaultDbConnection = "Server=127.0.0.1;" +
                                              "Port=5432;" +
                                              "Database=myDataBase;" +
                                              "User Id=myUsername;" +
                                              "Password=myPassword";
    
    public static string Get(string? jsonString)
    {
        if (jsonString is null) return DefaultDbConnection;
        var connection = JsonSerializer.Deserialize<ConnectionString>(jsonString);
        return connection is null ? DefaultDbConnection : connection.ToString();
    }

    public override string ToString()
    {
        return $"Host={Host};" +
               $"Port={Port};" +
               $"Database={Database};" +
               $"Username={Username};" +
               $"Password={Password};" +
               $"IncludeErrorDetail=true";
    }
}