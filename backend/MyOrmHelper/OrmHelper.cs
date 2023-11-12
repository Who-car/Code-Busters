using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Npgsql;

namespace MyOrmHelper;

public class OrmHelper<T>
{
    private readonly string _tableName;
    private readonly PropertyInfo[] _properties;
    private readonly NpgsqlConnection _connection;

    public OrmHelper(NpgsqlConnection connection)
    {
        var entityType = typeof(T);
        _tableName = $"{entityType.Name.ToLower()}s";
        _properties = entityType.GetProperties();
        _connection = connection;
    }

    private bool CheckConnection()
    {
        return _connection?.State == ConnectionState.Open;
    }

    public async Task<int> CreateTable(string name, params Column[] columns)
    {
        var checkCommand = _connection.CreateCommand();
        var checkCommandText = $"select table_name from information_schema.tables where table_name='{name}'";
        checkCommand.CommandText = checkCommandText;
        await using var reader = await checkCommand.ExecuteReaderAsync();

        if (reader.HasRows) return 0;
        
        await reader.CloseAsync();
        await _connection.CloseAsync();
        await _connection.OpenAsync();
        
        if (!columns.Any(x => x.PrimaryKey))
            throw new MissingPrimaryKeyException("table must contain primary key");
        if (columns.Count(x => x.PrimaryKey) > 1)
            throw new MissingPrimaryKeyException("more than 1 primary key is being put into table");
        if (columns
            .Where(x => x.ForeignKey)
            .Any(y => string.IsNullOrEmpty(y.ReferencesTable) && string.IsNullOrEmpty(y.ReferencesColumn)))
            throw new NullReferenceException("foreign key references no existing table");

        var querySet = string.Join(", ", columns.Select(x => $"{x.Name} {x.SqlType}{(x.PrimaryKey ? " primary key" : "" )}"));
        var constraints = columns.Any(x => x.ForeignKey) 
            ? string.Join(' ', columns
                .Where(x => x.ForeignKey)
                .Select(x => $", foreign key ({x.Name}) references public.{x.ReferencesTable}({x.ReferencesColumn})")) 
            : null;
        var commandText = $"create table {name}({querySet}{constraints})";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        return await command.ExecuteNonQueryAsync();
    }
    
    public async Task<TReturn> SearchWithParamsAsync<TReturn>(string columnName, object value) where TReturn: new()
    {
        if (!CheckConnection()) throw new ChannelClosedException();
        
        var querySet = _properties
            .Where(p => ConvertCase(p.Name) == columnName)
            .Select(p => $"{ConvertCase(p.Name)} = '{value}'");
        var commandText = $"select * from {_tableName} where {string.Join(" and ", querySet)}";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows || !await reader.ReadAsync()) throw new KeyNotFoundException();
        
        var instance = new TReturn();
        var props = typeof(TReturn).GetProperties();

        props.ToList().ForEach(p =>
        {
            var columnName = ConvertCase(p.Name);
            
            try
            {
                var value = reader[columnName];

                if (value != DBNull.Value)
                {
                    p.SetValue(instance, value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });

        return instance;
    }
    
    public async Task<int> InsertAsync(T entity, string? tableName = null)
    {
        if (!CheckConnection()) return -1;
        tableName ??= _tableName;
        var columns = new List<string>();
        var checkCommand = _connection.CreateCommand();
        var checkCommandText = $"select column_name from information_schema.columns where " +
                               $"table_schema='public' and " +
                               $"table_name='{tableName}'";
        checkCommand.CommandText = checkCommandText;
        await using var reader = await checkCommand.ExecuteReaderAsync();

        if (!reader.HasRows) return 0;
        while (await reader.ReadAsync())
        {
            columns.Add(reader[0].ToString());
        }
        
        await _connection.CloseAsync();
        await _connection.OpenAsync();
        
        var values = new List<object>();
        var insert = $"insert into {tableName}(" +
                     $"{string.Join(",", _properties
                         .Where(p =>
                         {
                             if (!columns.Contains(ConvertCase(p.Name))) return false;
                             var res = GetMyValue(p, entity);
                             if (res is null) return false;
                             values.Add(res);
                             return true; 
                         })
                         .Select(t => ConvertCase(t.Name)))})";
        var commandText = $"{insert} values('{string.Join("','", values)}')";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        return await command.ExecuteNonQueryAsync();
    }

    public int Update(T entity, string keyName, object keyValue)
    {
        if (!CheckConnection()) return -1;
        
        var setValues = _properties.ToDictionary(p => p.Name, p => $"'{p.GetValue(entity)}'");
        var setSql = string.Join(",", setValues.Select(pair=>$"{pair.Key}='{pair.Value}'"));
        var commandText = $"update {_tableName} set {setSql} where {keyName} = '{keyValue}'";
        
        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        return command.ExecuteNonQuery();
    }

    public int Delete(T entity)
    {
        if (!CheckConnection()) return -1;
        
        var querySet = _properties.Select(p => $"{p.Name} = '{p.GetValue(entity)}'");
        var commandText = $"delete from {_tableName} where {string.Join(" and ", querySet)}";
        
        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        return command.ExecuteNonQuery();
    }
    
    public List<T> LeftJoin<T, U>(
        Expression<Func<T, U, bool>> joinCondition,
        Expression<Func<T, U, dynamic>> resultSelector)
    {
        var leftTableName = typeof(T).Name;
        var rightTableName = typeof(U).Name;

        var joinTable = _connection.CreateCommand();
        joinTable.CommandText = $"SELECT * FROM {leftTableName} LEFT JOIN {rightTableName} ON {ParseJoinCondition(joinCondition)}";
        NpgsqlDataReader reader = joinTable.ExecuteReader();

        var results = new List<T>();

        while (reader.Read())
        {
            var leftObject = Activator.CreateInstance<T>();
            var rightObject = Activator.CreateInstance<U>();

            foreach (var property in leftObject.GetType().GetProperties())
            {
                property.SetValue(leftObject, reader[property.Name]);
            }

            foreach (var property in rightObject.GetType().GetProperties())
            {
                property.SetValue(rightObject, reader[property.Name]);
            }

            // Можно использовать resultSelector для проецирования результата
            dynamic dynamicResultSelector = resultSelector.Compile();
            dynamic result = dynamicResultSelector(leftObject, rightObject);

            results.Add(result);
        }

        reader.Close();
        return results;
    }

    private string ParseJoinCondition<T, U>(Expression<Func<T, U, bool>> joinCondition)
    {
        var body = (BinaryExpression)joinCondition.Body;
        var left = (MemberExpression)body.Left;
        var right = (MemberExpression)body.Right;

        return $"{typeof(T).Name}.{left.Member.Name} = {typeof(U).Name}.{right.Member.Name}";
    }

    public bool Exists(T entity, params string[]? constraints)
    {
        if (!CheckConnection()) return false;
        
        var querySet = constraints is null ? _properties.Select(p => $"{ConvertCase(p.Name)} = '{GetMyValue(p, entity)}'")
                : _properties.Where(p => constraints.Contains(ConvertCase(p.Name))).Select(p => $"{ConvertCase(p.Name)} = '{GetMyValue(p, entity)}'");
        var commandText = $"select * from {_tableName} where {string.Join(" and ", querySet)}";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        using var reader = command.ExecuteReader();

        return reader.HasRows && reader.Read();
    }

    private object? GetMyValue(PropertyInfo p, T entity)
    {
        if (p.GetValue(entity) is null) return null;
        if (p.GetCustomAttribute<ColumnAttribute>() is not null && 
            p.GetCustomAttribute<ColumnAttribute>().TypeName == "jsonb")
            return JsonSerializer.Serialize(p.GetValue(entity));
        return p.PropertyType.IsArray ? $"{{{string.Join(",", (((Array)p.GetValue(entity)!)!).OfType<object>())}}}" 
            : p.GetValue(entity);
    }

    private string ConvertCase(string input)
    {
        var output = new StringBuilder();
    
        for (int i = 0; i < input.Length; i++)
        {
            if (i > 0 && Char.IsUpper(input[i]))
            {
                output.Append('_');
                output.Append(Char.ToLower(input[i]));
            }
            else
            {
                output.Append(Char.ToLower(input[i]));
            }
        }
    
        return output.ToString();
    }
}