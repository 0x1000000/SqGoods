using System.Collections.Generic;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqExpress.QueryBuilders.Merge;

namespace SqGoods.DomainLogic.Repositories.Utils
{
    internal static class QueryHelper
    {
        public static async Task Update<T, TTable>(ISqDatabase database, IEnumerable<T> data, ISqModelUpdaterKey<T, TTable> updater, MergeUpdateMapping<TTable>? alsoSet = null)
            where TTable : TableBase, new()
        {
            var t = new TTable();
            var a = SqQueryBuilder.UpdateData(t, data).MapDataKeys(updater.GetUpdateKeyMapping).MapData(updater.GetUpdateMapping);

            if (alsoSet != null)
            {
                await a.AlsoSet(alsoSet).Exec(database);
            }
            else
            {
                await a.Exec(database);
            }
        }
    }
}