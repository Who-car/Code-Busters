namespace MyOrmHelper;

public static class TypeExtension
{
    public static string GetSqlType(this Type type)
    {
        var name = type.Name
            .Replace("[]", "")
            .Replace("Array", "");
        name = name switch
        {
            "Boolean" => "boolean",
            "Int32" => "integer",
            "Int16" => "smallint",
            "Byte" => "smallint",
            "Int64" => "bigint",
            "Single" => "real",
            "Double" => "double precision",
            "Decimal" => "numeric",
            "String" => "text",
            "Char" => "text",
            "Guid" => "uuid",
            "DateTime" => "timestamp without time zone",
            "DateTimeOffset" => "timestamp with time zone",
            "DateOnly" => "date",
            "TimeOnly" => "time without time zone",
            "TimeSpan" => "interval",
            "Json" => "jsonb",
            "JsonArray" => "jsonb[]",
            _ => throw new NotSupportedException()
        };
        return type.IsArray ? string.Concat(name, "[]") : name;
    }
}