using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqGoods.DomainLogic.Tables;
using static SqExpress.SqQueryBuilder;

namespace SqGoods.DomainLogic.DataAccess
{
    public interface IDatabaseManager
    {
        DomainLogicOptions Options { get; }

        bool IsInitialized { get; }

        Task<InitializationError?> Initialize(string? connectionString = null);

        Task WriteDbJsonDataToStream(Stream stream, int? pendingBytesLimit);

        InitializationError? LastError { get; }
    }

    internal class DatabaseManager: IDatabaseManager
    {
        private readonly SqlConnectionStorageFactory _connectionStorageFactory;

        private readonly SemaphoreSlim _semaphore = new(1);

        public DomainLogicOptions Options => this._connectionStorageFactory.Options;

        public bool IsInitialized { get; private set; }

        public InitializationError? LastError { get; private set; }

        public DatabaseManager(SqlConnectionStorageFactory connectionStorageFactory)
        {
            this._connectionStorageFactory = connectionStorageFactory;
        }

        public async Task<InitializationError?> Initialize(string? connectionString = null)
        {
            if (this.IsInitialized)
            {
                return null;
            }

            await this._semaphore.WaitAsync();

            if (this.IsInitialized)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(connectionString) && connectionString != this._connectionStorageFactory.Options.ConnectionString)
            {
                this._connectionStorageFactory.Options.ConnectionString = connectionString;
            }

            this.LastError = null;

            try
            {
                await using var cs = this._connectionStorageFactory.CreateStorage();

                try
                {
                    await cs.OpenConnectionAsync();
                }
                catch (Exception e)
                {
                    this.LastError = new InitializationError(InitializationError.InitializationErrorCode.Connection, null, e);
                    return this.LastError;
                }

                await using var database = cs.CreateDatabase();

                var declaredTables = AllTables.BuildAllTableList();

                var actualTables = await database.GetTables();

                var comparison = declaredTables.CompareWith(actualTables);

                if (comparison != null &&
                    (comparison.MissedTables is { Count: > 0 } || comparison.DifferentTables is { Count: > 0 }))
                {
                    var initializationError = await RecreateTables(database, declaredTables);
                    if (initializationError != null)
                    {
                        this.LastError = initializationError;
                        return this.LastError;
                    }
                }

                this.IsInitialized = true;
            }
            finally
            {
                this._semaphore.Release();
            }

            return null;
        }

        public async Task WriteDbJsonDataToStream(Stream stream, int? pendingBytesLimit)
        {
            await using var cs = this._connectionStorageFactory.CreateStorage();
            await using var database = cs.CreateDatabase();
            await using var jsonWriter = new Utf8JsonWriter(stream, options: new JsonWriterOptions { Indented = true });

            var allTables = AllTables.BuildAllTableList();

            jsonWriter.WriteStartObject();
            foreach (var table in allTables)
            {
                await ReadTableDataIntoJson(jsonWriter, database, table, pendingBytesLimit);
            }

            jsonWriter.WriteEndObject();
            await jsonWriter.FlushAsync();

            static async Task ReadTableDataIntoJson(Utf8JsonWriter writer, ISqDatabase database, TableBase table, int? pendingBytesLimit)
            {
                writer.WriteStartArray(table.FullName.AsExprTableFullName().TableName.Name);

                writer.WriteStartArray();
                foreach (var column in table.Columns)
                {
                    writer.WriteStringValue(column.ColumnName.Name);
                }

                writer.WriteEndArray();

                await Select(table.Columns)
                    .From(table)
                    .Query(database,
                        async r =>
                        {
                            writer.WriteStartArray();
                            foreach (var column in table.Columns)
                            {
                                var readAsString = column.ReadAsString(r);
                                writer.WriteStringValue(readAsString);
                            }
                            writer.WriteEndArray();
                            if (pendingBytesLimit.HasValue && writer.BytesPending >= pendingBytesLimit.Value)
                            {
                                await writer.FlushAsync();
                            }
                        });

                writer.WriteEndArray();
            }
        }


        private static async Task<InitializationError?> RecreateTables(ISqDatabase database, IReadOnlyList<TableBase> tables)
        {
            foreach (var table in tables.Reverse())
            {
                try
                {
                    await database.Statement(table.Script.DropIfExist());
                }
                catch (Exception e)
                {
                    return new InitializationError(
                        InitializationError.InitializationErrorCode.Recreation,
                        $"Dropping {table.FullName.AsExprTableFullName().TableName.Name}",
                        e);
                }
            }

            foreach (var table in tables)
            {
                try
                {
                    await database.Statement(table.Script.Create());
                }
                catch (Exception e)
                {
                    return new InitializationError(
                        InitializationError.InitializationErrorCode.Recreation,
                        $"Creating {table.FullName.AsExprTableFullName().TableName.Name}",
                        e);
                }
            }

            try
            {
                await InsertInitialData(database, tables);
            }
            catch (Exception e)
            {
                return new InitializationError(
                    InitializationError.InitializationErrorCode.InitDataInsertion,
                    null,
                    e);
            }

            return null;
        }

        private static async Task InsertInitialData(ISqDatabase database, IReadOnlyList<TableBase> tableBases)
        {
            var document = JsonDocument.Parse(InitialData.Json);

            var pending = new Dictionary<string, JsonElement>();

            using var enumerator = document.RootElement.EnumerateObject();
            if (!enumerator.MoveNext())
            {
                throw new Exception("Enumerator is empty");
            }

            foreach (var table in tableBases)
            {
                var tableName = table.FullName.AsExprTableFullName().TableName.Name;
                JsonElement element;

                if (enumerator.Current.Name != tableName && pending.TryGetValue(tableName, out var e))
                {
                    element = e;
                }
                else
                {
                    while (enumerator.Current.Name != tableName)
                    {
                        pending.Add(enumerator.Current.Name, enumerator.Current.Value);
                        if (!enumerator.MoveNext())
                        {
                            throw new Exception("Enumerator is empty");
                        }
                    }

                    element = enumerator.Current.Value;
                }

                await InsertTableData(database, table, element);
            }

            static async Task InsertTableData(ISqDatabase database, TableBase table, JsonElement element)
            {
                var columnsDict = table.Columns.ToDictionary(i => i.ColumnName.Name, i => i);

                List<string?> jsonColRow = element//null means that col does not exist in the db table (was removed?)
                    .EnumerateArray()
                    .First()
                    .EnumerateArray()
                    .Select(c => c.GetString() ?? throw new Exception("String is expected here"))
                    .Select(colName=> columnsDict.ContainsKey(colName) ? colName : null)
                    .ToList();

                var tableColumns = jsonColRow
                    .Where(colName=> colName != null)
                    .Select(colName => table.Columns.First(tableColumn =>
                        string.Equals(tableColumn.ColumnName.Name, colName, StringComparison.CurrentCultureIgnoreCase)))
                    .ToList();

                var rowsEnumerable = element
                    .EnumerateArray()
                    .Skip(1)
                    .Select(e =>
                        e.EnumerateArray()
                            .Where((_, i)=> jsonColRow[i] != null)
                            .Select((c, i) =>
                                columnsDict[jsonColRow[i]!]
                                    .FromString(c.ValueKind == JsonValueKind.Null ? null : c.GetString()))
                            .ToList());

                var insertExpr = IdentityInsertInto(table, tableColumns).Values(rowsEnumerable);
                if (!insertExpr.Insert.Source.IsEmpty)
                {
                    await insertExpr.Exec(database);
                }
            }
        }
    }


    public record InitializationError(
        InitializationError.InitializationErrorCode ErrorCode,
        string? Comments,
        Exception? Exception)
    {

        public string CreateMessage()
        {
            string message;
            switch (this.ErrorCode)
            {
                case InitializationErrorCode.Connection:
                    message =
                        "Could not to connect to the specified database. Make sure that the configuration file contains a correct connection string.";
                    break;
                case InitializationErrorCode.Recreation:
                    message =
                        "Could not to recreate database.";
                    break;
                case InitializationErrorCode.InitDataInsertion:
                    message =
                        "Could not insert the initial data.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrEmpty(this.Comments))
            {
                message += " " + this.Comments;
            }

            return message;
        }

        public enum InitializationErrorCode
        {
            Connection,
            Recreation,
            InitDataInsertion
        }
    }
}