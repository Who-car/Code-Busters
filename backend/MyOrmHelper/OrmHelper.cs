﻿using System.Collections;
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

    public async Task<int> CreateTableAsync(string name, CancellationToken token, params Column[] columns)
    {
        var checkCommand = _connection.CreateCommand();
        var checkCommandText = $"select table_name from information_schema.tables where table_name='{name}'";
        checkCommand.CommandText = checkCommandText;
        await using var reader = await checkCommand.ExecuteReaderAsync(token);

        if (reader.HasRows) return 0;
        await RestoreConnectionAsync(token);
        
        if (!columns.Any(x => x.PrimaryKey))
            throw new MissingPrimaryKeyException("table must contain primary key");
        if (columns.Count(x => x.PrimaryKey) > 1)
            throw new MissingPrimaryKeyException("more than 1 primary key is being put into table");
        if (columns
            .Where(x => x.ForeignKey)
            .Any(y => string.IsNullOrEmpty(y.ReferencesTable) && string.IsNullOrEmpty(y.ReferencesColumn)))
            throw new NullReferenceException("foreign key references no existing table");

        var querySet = string.Join(", ", 
            columns.Select(x => $"{x.Name} {x.SqlType}{(x.PrimaryKey ? " primary key" : "" )}"));
        var constraints = columns.Any(x => x.ForeignKey) 
            ? string.Join(' ', columns
                .Where(x => x.ForeignKey)
                .Select(x => $", foreign key ({x.Name}) references public.{x.ReferencesTable}({x.ReferencesColumn})")) 
            : null;
        var commandText = $"create table {name}({querySet}{constraints})";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        return await command.ExecuteNonQueryAsync(token);
    }
    
    public async Task<bool> ExistsAsync(T entity, CancellationToken token, string? tableName = null)
    {
        if (!CheckConnection()) return false;
        tableName ??= _tableName;
        
        var querySet = 
            _properties
                .Select(p => 
                    $"{ConvertCase(p.Name)} = '{GetMyValue(p, entity)}'");
        var commandText = $"select * from {tableName} where {string.Join(" and ", querySet)}";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        await using var reader = await command.ExecuteReaderAsync(token);

        return reader.HasRows;
    }
    
    public async Task<TReturn> FindAsync<TReturn>(IEnumerable<(string column, object value)> parameters, CancellationToken token, string? tableName = null) 
        where TReturn: new()
    {
        if (!CheckConnection()) throw new ChannelClosedException();
        tableName ??= _tableName;

        var querySet = 
            parameters
                .Select(pair => $"{pair.column}='{pair.value}'")
                .ToString();
        var commandText = $"select * from {tableName} where {string.Join(" and ", querySet)}";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        await using var reader = await command.ExecuteReaderAsync(token);

        if (!reader.HasRows || !await reader.ReadAsync(token)) 
            throw new KeyNotFoundException($"No {typeof(TReturn).Name} ({querySet}) found");
        
        var instance = new TReturn();
        typeof(TReturn)
            .GetProperties()
            .ToList()
            .ForEach(p =>
            {
                var columnName = ConvertCase(p.Name);
                
                try
                {
                    p.SetValue(instance, reader[columnName]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });

        return instance;
    }
    
    public async Task<int> InsertAsync(T entity, CancellationToken token, string? tableName = null)
    {
        if (!CheckConnection()) return -1;
        tableName ??= _tableName;
        
        var columns = new List<string>();
        var checkCommand = _connection.CreateCommand();
        var checkCommandText = $"select column_name from information_schema.columns where " +
                               $"table_schema='public' and " +
                               $"table_name='{tableName}'";
        checkCommand.CommandText = checkCommandText;
        await using var reader = await checkCommand.ExecuteReaderAsync(token);

        if (!reader.HasRows) return 0;
        while (await reader.ReadAsync(token))
            columns.Add(reader[0].ToString()!);

        await RestoreConnectionAsync(token);
        var values = new List<object>();
        var insert = $"insert into {tableName}(" +
                     $"{string.Join(",", _properties
                         .Where(p => columns.Contains(ConvertCase(p.Name)))
                         .Where(p =>
                         {
                             var res = GetMyValue(p, entity);
                             if (res is null) return false;
                             values.Add(res);
                             return true; 
                         })
                         .Select(t => ConvertCase(t.Name)))})";
        var commandText = $"{insert} values('{string.Join("','", values)}')";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        return await command.ExecuteNonQueryAsync(token);
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
    
    public List<TLeft> LeftJoin<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> joinCondition, Expression<Func<TLeft, TRight, dynamic>> resultSelector)
    {
        var leftTableName = $"{typeof(TLeft).Name.ToLower()}s";
        var rightTableName = $"{typeof(TRight).Name.ToLower()}s";

        var joinTable = _connection.CreateCommand();
        joinTable.CommandText = $"select * from {leftTableName} left join {rightTableName} on {ParseJoinCondition(joinCondition)}";
        var reader = joinTable.ExecuteReader();

        var results = new List<TLeft>();

        while (reader.Read())
        {
            var leftObject = Activator.CreateInstance<TLeft>();
            var rightObject = Activator.CreateInstance<TRight>();

            foreach (var property in leftObject!.GetType().GetProperties())
                property.SetValue(leftObject, reader[property.Name]);

            foreach (var property in rightObject!.GetType().GetProperties())
                property.SetValue(rightObject, reader[property.Name]);
            
            var dynamicResultSelector = resultSelector.Compile();
            var result = dynamicResultSelector(leftObject, rightObject);

            results.Add(result);
        }

        reader.Close();
        return results;
    }

    private string ParseJoinCondition<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> joinCondition)
    {
        var body = (BinaryExpression)joinCondition.Body;
        var left = (MemberExpression)body.Left;
        var right = (MemberExpression)body.Right;

        return $"{typeof(TLeft).Name}.{left.Member.Name} = {typeof(TRight).Name}.{right.Member.Name}";
    }

    private object? GetMyValue(PropertyInfo p, T entity)
    {
        var value = p.GetValue(entity);
        var columnType = p.GetCustomAttribute<ColumnAttribute>();
        
        if (value is null) 
            return null;
        
        if (columnType is not null && columnType.TypeName == "jsonb")
            return JsonSerializer.Serialize(value);
        
        return p.PropertyType.IsArray 
            ? $"{{{string.Join(",", ((Array)value).OfType<object>())}}}" 
            : value;
    }

    private string ConvertCase(string input)
    {
        return input
            .Select(s => 
                char.IsUpper(s) 
                    ? $"_{char.ToLower(s)}" 
                    : $"{s}")
            .ToString()!;
    }
    
    private bool CheckConnection()
    {
        return _connection.State == ConnectionState.Open;
    }

    private async Task RestoreConnectionAsync(CancellationToken token)
    {
        await _connection.CloseAsync();
        await _connection.OpenAsync(token);
    }
}