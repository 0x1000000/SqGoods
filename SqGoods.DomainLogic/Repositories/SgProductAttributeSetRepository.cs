using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Tables;

namespace SqGoods.DomainLogic.Repositories
{
    public interface ISgProductAttributeSetRepository
    {
        Task Merge(IReadOnlyList<SgProductAttributeSet> data);
        Task DeleteByProduct(IReadOnlyList<Guid> productIds);
    }

    internal class SgProductAttributeSetRepository : ISgProductAttributeSetRepository
    {
        private readonly ISqDatabase _database;

        public SgProductAttributeSetRepository(ISqDatabase database)
        {
            this._database = database;
        }

        public async Task Merge(IReadOnlyList<SgProductAttributeSet> data)
        {
            await SqQueryBuilder
                .MergeDataInto(AllTables.GetProductAttributeSet(), data)
                .MapDataKeys(SgProductAttributeSet.GetMapping)
                .WhenNotMatchedByTargetThenInsert()
                .WhenNotMatchedBySourceThenDelete(
                    t => t.ProductId.In(data.Select(d => d.ProductId).Distinct().ToList()))
                .Done()
                .Exec(this._database);
        }

        public async Task DeleteByProduct(IReadOnlyList<Guid> productIds)
        {
            if (productIds.Count < 1)
            {
                return;
            }

            var tbl = AllTables.GetProductAttributeSet();

            await SqQueryBuilder.Delete(tbl).Where(tbl.ProductId.In(productIds)).Exec(this._database);
        }
    }
}