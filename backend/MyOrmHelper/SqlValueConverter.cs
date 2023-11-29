using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MyOrmHelper;

public static class SqlValueConverter
{
    public static string? ConvertToSql(PropertyInfo property, object instance)
    {
        var value = property.GetValue(instance);
        
        if (value is null) 
            return null;
        
        return property.PropertyType.IsArray 
            ? $"{{{string.Join(",", ((Array)value).OfType<object>())}}}" 
            : Convert.ToString(value, CultureInfo.InvariantCulture);
    }
    
    public static object ConvertFromSql(PropertyInfo property, object value)
    {
        if (property.PropertyType == typeof(JsonArray))
            return JsonSerializer.Deserialize(value.ToString(), property.PropertyType);
        if (property.PropertyType.IsEnum)
            return Enum.Parse(property.PropertyType, value.ToString());
        return value;
    }
}