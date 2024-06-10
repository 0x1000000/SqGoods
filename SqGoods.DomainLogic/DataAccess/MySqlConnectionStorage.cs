using System.Threading.Tasks;
using MySqlConnector;
using SqExpress.DataAccess;
using SqExpress.SqlExport;

namespace SqGoods.DomainLogic.DataAccess
{
    internal class MySqlConnectionStorage : ISqlConnectionStorage
    {
        private readonly MySqlConnection _connection;

        private static readonly MySqlExporter PgSqlExporter = MySqlExporter.Default;

        public MySqlConnectionStorage(string connectionString)
        {
            this._connection = new MySqlConnection(connectionString);
        }

        public ISqDatabase CreateDatabase()
        {
            return new SqDatabase<MySqlConnection>(
                connection: this._connection,
                commandFactory: (conn, sql) => new MySqlCommand(commandText: sql, connection: conn),
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