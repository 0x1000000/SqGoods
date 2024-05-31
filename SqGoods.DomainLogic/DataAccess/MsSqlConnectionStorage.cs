using System.Data.SqlClient;
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

        public void OpenConnection()
        {
            this._connection.Open();
        }

        public void Dispose()
        {
            this._connection.Dispose();
        }
    }
}