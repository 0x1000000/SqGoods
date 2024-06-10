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
    public interface ISgAttributeSetRepository
    {
        Task Create(IReadOnlyList<SgAttributeItem> items);

        Task Delete(IReadOnlyList<Guid> attributeIds);

        Task Merge(IReadOnlyList<SgAttributeItem> attributesItems);
    }

    internal class SgAttributeSetRepository : ISgAttributeSetRepository
    {
        private readonly ISqDatabase _database;

        public SgAttributeSetRepository(ISqDatabase database)
        {
            this._database = database;
        }

        public Task Create(IReadOnlyList<SgAttributeItem> items)
        {
            if (items.Count < 1)
            {
                return Task.CompletedTask;
            }
            var tbl = AllTables.GetAttributeSet();
            return SqQueryBuilder.InsertDataInto(tbl, items).MapData(SgAttributeItem.GetMapping).Exec(this._database);
        }

        public Task Delete(IReadOnlyList<Guid> attributeIds)
        {
            var tbl = AllTables.GetAttributeSet();
            return SqQueryBuilder.Delete(tbl).Where(tbl.AttributeId.In(attributeIds)).Exec(this._database);
        }

        public async Task Merge(IReadOnlyList<SgAttributeItem> attributesItems)
        {
            if (attributesItems.Count < 1)
            {
                return;
            }

            var allAttributes = attributesItems.Select(a => a.AttributeId).Distinct().ToList();

            //Removing products
            var tProductAttributeSet = AllTables.GetProductAttributeSet();
            var tAttributeSet = AllTables.GetAttributeSet();

            var (tran, _) = await this._database.BeginTransactionOrUseExistingAsync();
            await using (tran)
            {

                await SqQueryBuilder
                    .Delete(tProductAttributeSet)
                    .From(tProductAttributeSet)
                    .InnerJoin(tAttributeSet, on: tAttributeSet.AttributeSetId == tProductAttributeSet.AttributeSetId)
                    .Where(
                        tAttributeSet.AttributeId.In(allAttributes) &
                        !tProductAttributeSet.AttributeSetId.In(attributesItems.Select(i => i.AttributeSetId).ToList())
                    )
                    .Exec(this._database);


                await SqQueryBuilder
                    .MergeDataInto(AllTables.GetAttributeSet(), attributesItems)
                    .MapDataKeys(SgAttributeItem.GetUpdateKeyMapping)
                    .MapData(SgAttributeItem.GetUpdateMapping)
                    .WhenMatchedThenUpdate()
                    .WhenNotMatchedByTargetThenInsert()
                    .WhenNotMatchedBySourceThenDelete(t => t.AttributeId.In(allAttributes))
                    .Done()
                    .Exec(this._database);

                await tran.CommitAsync();
            }
        }
    }
}