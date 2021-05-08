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
    public interface ISgProductAttributeRepository
    {
        Task Merge(IReadOnlyList<SgProductAttribute> data);
        Task Delete(IReadOnlyList<(Guid ProductId, Guid AttributeId)> productAttributeIds);
        Task DeleteByProduct(IReadOnlyList<Guid> productIds);
        Task DeleteByAttribute(IReadOnlyList<Guid> attributeIds);
    }

    internal class SgProductAttributeRepository : ISgProductAttributeRepository
    {
        private readonly ISqDatabase _database;

        public SgProductAttributeRepository(ISqDatabase database)
        {
            this._database = database;
        }

        public async Task Merge(IReadOnlyList<SgProductAttribute> data)
        {
            await SqQueryBuilder
                .MergeDataInto(AllTables.GetProductAttribute(), data)
                .MapDataKeys(SgProductAttribute.GetUpdateKeyMapping)
                .MapData(SgProductAttribute.GetUpdateMapping)
                .WhenMatchedThenUpdate()
                .WhenNotMatchedByTargetThenInsert()
                .WhenNotMatchedBySourceThenDelete(
                    t => t.ProductId.In(data.Select(d => d.ProductId).Distinct().ToList()))
                .Done()
                .Exec(this._database);
        }

        public async Task Delete(IReadOnlyList<(Guid ProductId, Guid AttributeId)> productAttributeIds)
        {
            if (productAttributeIds.Count < 1)
            {
                return;
            }

            var tbl = AllTables.GetProductAttribute();

            await SqQueryBuilder
                .Delete(tbl)
                .Where(productAttributeIds.Select(pa=> tbl.ProductId == pa.ProductId & tbl.AttributeId == pa.AttributeId).JoinAsOr())
                .Exec(this._database);
        }

        public async Task DeleteByProduct(IReadOnlyList<Guid> productIds)
        {
            if (productIds.Count < 1)
            {
                return;
            }

            var tbl = AllTables.GetProductAttribute();

            await SqQueryBuilder.Delete(tbl).Where(tbl.ProductId.In(productIds)).Exec(this._database);
        }

        public async Task DeleteByAttribute(IReadOnlyList<Guid> attributeIds)
        {
            if (attributeIds.Count < 1)
            {
                return;
            }

            var tbl = AllTables.GetProductAttribute();

            await SqQueryBuilder.Delete(tbl).Where(tbl.AttributeId.In(attributeIds)).Exec(this._database);
        }
    }
}