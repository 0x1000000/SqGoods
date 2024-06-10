using System.Data.SqlClient;
using System.Threading.Tasks;
using SqExpress.DataAccess;
using SqExpress.SqlExport;

namespace SqGoods.DomainLogic.DataAccess
{
    internal class MsSqlConnectionStorage : ISqlConnectionStorage
    {
        private readonly SqlConnection _connection;

        public MsSqlConnectionStorage(string connectionString)
        {
            this._connection = new SqlConnection(connectionString);
        }

        public ISqDatabase CreateDatabase()
        {
            return new SqDatabase<SqlConnection>(
                connection: this._connection,
                commandFactory: (conn, sql) => new SqlCommand(cmdText: sql, connection: conn),
                sqlExporter: TSqlExporter.Default);
        }

        public Task OpenConnectionAsync()
        {
            return this._connection.OpenAsync();
        }

        public void Dispose()
        {
            this._connection.DisposeAsync();
        }

        public ValueTask DisposeAsync()
        {
            return this._connection.DisposeAsync();
        }
    }
}