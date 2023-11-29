namespace MyOrmHelper;

public static class StringExtension
{
    public static string ToSql(this string input)
    {
        return string.Join(
            "", 
            input.Select(s => 
                char.IsUpper(s) 
                    ? input.IndexOf(s) == 0 
                        ? $"{char.ToLower(s)}" 
                        : $"_{char.ToLower(s)}" 
                    : $"{s}"));
    }
}