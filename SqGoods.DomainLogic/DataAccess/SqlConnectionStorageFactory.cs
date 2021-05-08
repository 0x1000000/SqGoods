using System;

namespace SqGoods.DomainLogic.DataAccess
{
    internal class SqlConnectionStorageFactory
    {
        public DomainLogicOptions Options { get; }

        public SqlConnectionStorageFactory(DomainLogicOptions options)
        {
            this.Options = options;
        }

        public ISqlConnectionStorage CreateStorage()
        {
            switch (this.Options.DatabaseType)
            {
                case DatabaseType.MsSql:
                    return new MsSqlConnectionStorage(this.Options.ConnectionString);
                case DatabaseType.PgSql:
                    return new PgSqlConnectionStorage(this.Options.ConnectionString);
                case DatabaseType.MySql:
                    return new MySqlConnectionStorage(this.Options.ConnectionString);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}