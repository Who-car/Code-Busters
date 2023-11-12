namespace MyOrmHelper;

public class Column
{
    public string Name { get; }
    public string SqlType { get; }
    public bool PrimaryKey { get; }
    public bool ForeignKey { get; }
    public string? ReferencesTable { get; }
    public string? ReferencesColumn { get; }

    public Column(string name, Type type, bool isPrimary = false)
    {
        Name = string.Join('_', name.Split(' ')).ToLower();
        SqlType = type.GetSqlType();
        PrimaryKey = isPrimary;
    }

    public Column(string name, Type type, string referencedTable, string referencedColumn)
    {
        Name = string.Join('_', name.Split(' ')).ToLower();
        SqlType = type.GetSqlType();
        PrimaryKey = false;
        ForeignKey = true;
        ReferencesTable = referencedTable;
        ReferencesColumn = referencedColumn;
    }
}