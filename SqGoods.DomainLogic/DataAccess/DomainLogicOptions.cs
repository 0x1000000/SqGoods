namespace SqGoods.DomainLogic.DataAccess
{
    public class DomainLogicOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public DatabaseType DatabaseType = DatabaseType.MsSql;
    }

    public enum DatabaseType
    {
        MsSql,
        PgSql,
        MySql
    }
}