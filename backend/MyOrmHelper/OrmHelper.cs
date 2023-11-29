using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Channels;
using Npgsql;

namespace MyOrmHelper;

public class OrmHelper
{
    private readonly Dictionary<Type, PropertyInfo[]> _properties = new();
    private NpgsqlConnection _connection;

    public void ConfigureConnection(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CreateTableAsync(string name, CancellationToken token, params Column[] columns)
    {
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
        var commandText = $"create table if not exists {name}({querySet}{constraints})";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        return await command.ExecuteNonQueryAsync(token);
    }
    
    public async Task<int> CreateTypeAsync(Type createdType, string baseSqlType, CancellationToken token)
    {
        if (!CheckConnection()) return -1;

        var typeName = createdType.Name.ToSql();
        var checkTypeExistsCommand = _connection.CreateCommand();
        checkTypeExistsCommand.CommandText = $"SELECT EXISTS(SELECT 1 FROM pg_type WHERE typname = '{typeName}')";
        var typeExists = (bool) (await checkTypeExistsCommand.ExecuteScalarAsync(token))!;
    
        if (typeExists) return 0;

        var enumValues = createdType.GetEnumNames().Select(s => $"'{s}'");
        var createTypeCommand = _connection.CreateCommand();
        createTypeCommand.CommandText = $"CREATE TYPE {typeName} AS {baseSqlType.ToUpper()}({string.Join(", ", enumValues)})";
    
        return await createTypeCommand.ExecuteNonQueryAsync(token);
    }

    public async Task<List<TReturn>> SelectAsync<TReturn>(CancellationToken token, string tableName, string? cursor = null, int? limit = null) 
        where TReturn: new()
    {
        if (!CheckConnection()) 
            throw new ChannelClosedException();
        
        var command = _connection.CreateCommand();
        var constraints = limit is null
            ? ""
            : $" where id > '{cursor}' order by id limit {limit}";
        command.CommandText = $"select * from {tableName}{constraints}";
    
        var reader = await command.ExecuteReaderAsync(token);
        var items = new List<TReturn>();
        
        if (!_properties.ContainsKey(typeof(TReturn)))
            _properties.Add(typeof(TReturn), typeof(TReturn).GetProperties());
    
        while (await reader.ReadAsync(token))
        {
            var item = new TReturn();
        
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var fieldName = reader.GetName(i);
                var property = _properties[typeof(TReturn)].FirstOrDefault(p => p.Name.ToSql() == fieldName);

                if (property == null) continue;

                var value = SqlValueConverter.ConvertFromSql(property, reader.GetValue(i));
                property.SetValue(item, value);
            }
        
            items.Add(item);
        }
    
        await reader.CloseAsync();
        return items;
    }
    
    public async Task<TReturn> FindAsync<TReturn>(CancellationToken token, string tableName, IEnumerable<(string column, object value)> parameters) 
        where TReturn: new()
    {
        if (!CheckConnection()) throw new ChannelClosedException();
        
        var querySet = string.Join(" and ", parameters
            .Select(pair => $"{pair.column}='{pair.value}'"));
        var commandText = $"select * from {tableName} where {querySet}";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        await using var reader = await command.ExecuteReaderAsync(token);

        if (!reader.HasRows || !await reader.ReadAsync(token)) 
            throw new KeyNotFoundException($"No {typeof(TReturn).Name} ({querySet}) found");
        
        if (!_properties.ContainsKey(typeof(TReturn)))
            _properties.Add(typeof(TReturn), typeof(TReturn).GetProperties());
        
        var item = new TReturn();
        
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var fieldName = reader.GetName(i);
            var property = _properties[typeof(TReturn)].FirstOrDefault(p => p.Name.ToSql() == fieldName);

            if (property == null) continue;

            var value = SqlValueConverter.ConvertFromSql(property, reader.GetValue(i));
            property.SetValue(item, value);
        }

        return item;
    }
    
    public async Task<int> InsertAsync<T>(CancellationToken token, string tableName, T entity)
    {
        if (!CheckConnection()) return -1;
        
        var columns = new List<string>();
        var values = new List<object>();
        
        if (!_properties.ContainsKey(typeof(T)))
            _properties.Add(typeof(T), typeof(T).GetProperties());
        
        foreach (var propertyInfo in _properties[typeof(T)])
        {
            var value = SqlValueConverter.ConvertToSql(propertyInfo, entity);
            if (value is null) continue;
            columns.Add(propertyInfo.Name.ToSql());
            values.Add(value);
        }
        
        var commandText = $"insert into {tableName} ({string.Join(",", columns)}) values('{string.Join("','", values)}')";

        var command = _connection.CreateCommand();
        command.CommandText = commandText;
        return await command.ExecuteNonQueryAsync(token);
    }
    
    public async Task<List<TResult>> LeftJoinAsync<TLeft, TRight, TResult>(
        CancellationToken token, 
        string leftTableName, 
        string rightTableName, 
        Expression<Func<TLeft, TRight, bool>> joinCondition, 
        Expression<Func<TLeft, TRight, TResult>> resultSelector, 
        Expression<Func<TLeft, bool>>? constraints = null) 
        where TLeft: new()
        where TRight: new()
    {
        var joinTable = _connection.CreateCommand();
        joinTable.CommandText = $"select * from {leftTableName} left join {rightTableName} on {ParseJoinCondition(joinCondition)}{ParseConstraints(constraints)}";
        var reader = await joinTable.ExecuteReaderAsync(token);

        var results = new List<TResult>();
        var dynamicResultSelector = resultSelector.Compile();

        while (await reader.ReadAsync(token))
        {
            var leftObject = new TLeft();
            var rightObject = new TRight();

            foreach (var property in leftObject!.GetType().GetProperties())
                property.SetValue(leftObject, reader[property.Name]);

            foreach (var property in rightObject!.GetType().GetProperties())
                property.SetValue(rightObject, reader[property.Name]);
            
            var result = dynamicResultSelector(leftObject, rightObject);
            results.Add(result);
        }

        await reader.CloseAsync();
        return results;
    }

    private string ParseJoinCondition<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> joinCondition)
    {
        var body = (BinaryExpression)joinCondition.Body;
        var left = (MemberExpression)body.Left;
        var right = (MemberExpression)body.Right;

        return $"{typeof(TLeft).Name}s.{left.Member.Name} = {typeof(TRight).Name}es.{right.Member.Name}";
    }
    
    private string ParseConstraints<TLeft>(Expression<Func<TLeft, bool>>? constraint)
    {
        if (constraint is null)
            return "";
        
        var body = (BinaryExpression)constraint.Body;
        var left = (MemberExpression)body.Left;
        var right = (MemberExpression)body.Right;

        return $"{typeof(TLeft).Name}s.{left.Member.Name} = {right}";
    }
    
    private bool CheckConnection()
    {
        return _connection.State == ConnectionState.Open;
    }
}