using System.Threading.Tasks;
using Npgsql;
using SqExpress.DataAccess;
using SqExpress.SqlExport;

namespace SqGoods.DomainLogic.DataAccess
{
    internal class PgSqlConnectionStorage : ISqlConnectionStorage
    {
        private readonly NpgsqlConnection _connection;

        private static readonly PgSqlExporter PgSqlExporter = new PgSqlExporter(
            SqlBuilderOptions.Default.WithSchemaMap(new[] { new SchemaMap("dbo", "public") }));


        public PgSqlConnectionStorage(string connectionString)
        {
            this._connection = new NpgsqlConnection(connectionString);
        }

        public ISqDatabase CreateDatabase()
        {
            return new SqDatabase<NpgsqlConnection>(
                connection: this._connection,
                commandFactory: (conn, sql) => new NpgsqlCommand(cmdText: sql, connection: conn),
                sqlExporter: PgSqlExporter);
        }

        public Task OpenConnectionAsync()
        {
            return this._connection.OpenAsync();
        }

        public void Dispose()
        {
            this._connection.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return this._connection.DisposeAsync();
        }
    }
}