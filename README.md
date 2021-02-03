# Data dude
Data Dude is a library to help create test data based on the database schema. Data dude is inspired by [CaptainData](https://github.com/mattiasnordqvist/Captain-Data) but its internals and strategy differs a bit which is supposed to make it more extendable and configurable.

Get started with Data dude:
```csharp
await new DataDude()
    .Insert("Department")
    .Insert("Employee", new { Name = "John Doe" }, new { Name = "Jane Doe" })
    .Go(connection);
```

## Concepts
Data dude is at its core an instruction handler, which means that it handles instructions. Built in are insertions and execution handling.

### Inserts
Inserts are what Data dude does best. It can insert rows based on schema-knowledge and configurable handling. That way you should be able to just specify the data you acually care about. Other column values should be taken care of by the dude. With its default configuration it should be able to handle most cases, but when you encounter edge cases or want to re-configure the default behavior, there are some knobs to tweak:

#### Automatic Foreign Keys
This is a concept where Data dude can utilize the schema information and fill in foreign key column values based on previos inserts. This functionality can be enabled like so:

```csharp
await new DataDude()
    .EnableAutomaticForeignKeys()
    .Insert("A")
    .Insert("B")
    .Go(connection);
```
In this example, if table B has a foreign key to table A, then Data dude will automatically set the corresponding foreign keys so a row can be inserted to table B without specifically specifying these keys manually.

This concept can be taken taken even a bit further by instructing Data dude to dynamically add missing foreign keys, like so:

```csharp
await new DataDude()
    .EnableAutomaticForeignKeys(x => x.AddMissingForeignKeys = true)
    .Insert("B")
    .Go(connection);
```
In this example, if table B has a foreign key to table A, then Data dude will automatically generate an insert instruction for table A before the table B -instruction. Foreign keys will then be handled in the same way as in the example above.

To make matters a bit more complicated, you might not always want foreign keys to automatically be inserted. You might for instance not want nullable or recursive foreign keys inserted. The strategy can be configured by setting the `DependencyTraversalStrategy` configuration like so:

```csharp
new Dude()
    .EnableAutomaticForeignKeys(x =>
    {
        x.AddMissingForeignKeys = true;
        x.DependencyTraversalStrategy = DependencyTraversalStrategy.SkipNullableForeignKeys
    })
```

Data dude comes preconfigured with 3 dependency traversal strategy options: 
- `FollowAllForeignKeys`: Default strategy will attempt to generate inserts for all foreign keys
- `SkipRecursiveForeignKeys`: Will not attempt to generate inserts for foreign keys where the table has a reference to itself
- `SkipNullableForeignKeys`: Will not attempt to generate inserts for nullable foreign keys
If you want more control you can create your own class that implements IDependencyTraversalStrategy and plug it in like above.

#### InsertValueProviders
They provide default column values for the usual data types before an insert is made. If you want to add your own default values, you can configure them using shorthand like this:
```csharp
await new DataDude()
    .ConfigureCustomColumnValue((column, value) => 
    {
        if (column.Name == "Active")
        {
            value.Set(new ColumnValue(true));
        }
    })
    .Insert("Employee")
    .Go(connection);
```
... or if you have more complex logic you can create your own value provider and add it like so:
```csharp
public class ActiveUserValueProvider : IValueProvider
{
    public void Process(ColumnInformation column, ColumnValue previousValue)
    {
        if (previousValue.Type == ColumnValueType.NotSet &&
            column.DataType == "bit" &&
            column.Table.Name == "User" &&
            column.Name == "Active")
        {
            previousValue.Set(new ColumnValue(true));
        }
    }
}

dude.ConfigureInsert(x => x.InsertValueProviders.Add(new ActiveUserValueProvider()));
```

#### InsertInterceptors
These are executed before and after inserts (but after value providers have been executed). By default only one interceptor, `IdentityInsertInterceptor` is configured (which handles setting and resetting identity inserts when needed). Another one can be configured using `.EnableAutomaticForeignKeys()` which is an interceptor that attempts to set foreign keys for you based on previously inserted valued. You can also add your own interceptor like so:
```csharp
public class AlwaysTrue : IInsertInterceptor
{
    public Task OnInsert(InsertStatement statement, InsertContext context, IDbConnection connection, IDbTransaction? transaction = null)
    {
        foreach (var (column, value) in statement.Data)
        {
            if (column.DataType == "bit")
            {
                value.Set(new ColumnValue(true));
            }
        }

        return Task.CompletedTask;
    }

    public Task OnInserted(InsertedRow insertedRow, InsertStatement statement, InsertContext context, IDbConnection connection, IDbTransaction? transaction = null)
        => Task.CompletedTask;
}

dude.ConfigureInsert(x => x.InsertInterceptors.Add(new AlwaysTrue()));
```

#### InsertRowHandlers
These are the handlers that do the actual insertion. Only the first insert handler that can handle the insert statement gets to execute the insert and return the inserted row. By default there are two insert handlers:
- `IdentityInsertRowHandler` handles insertion when there is only one primary key with identity. The inserted row retrieved using `SCOPE_IDENTITY()`
- `GeneratingInsertRowHandler` handes inserts by generating unspecified primary keys when possible*

*) Uses specified default column values if possible, if not available will generate keys if data type is one if `uniqueidentifier`, `nvarchar`, `varchar`, `shortint`, `int`, `bigint`.

There is one other insert handler available, `OutputInsertRowHandler` which uses `OUTPUT identity.*` to get ahold of the inserted row, not enabled by default since the default two should cover most bases and since it it also needs to disable any table triggers in order to perform the insert, which might not be what you want. If you do want to use it, it can be plugged in like this:
```csharp
dude.ConfigureInsert(x => x.InsertRowHandlers.Add(new OutputInsertRowHandler()));
```

## Dependencies
- [Dapper](https://github.com/StackExchange/Dapper) is used for internal data access
