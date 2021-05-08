using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Tables;
using static SqExpress.SqQueryBuilder;

namespace SqGoods.DomainLogic.Repositories
{
    public interface ISgCategoryAttributeRepository
    {
        Task Create(IReadOnlyCollection<SgCategoryAttributeMandatory> categoryAttributes);

        Task Merge(IReadOnlyCollection<SgCategoryAttributeMandatory> categoryAttributes);

        Task Update(IReadOnlyCollection<SgCategoryAttributeOrder> categoryAttributes);
    }

    internal class SgCategoryAttributeRepository : ISgCategoryAttributeRepository
    {
        private readonly ISqDatabase _database;

        public SgCategoryAttributeRepository(ISqDatabase database)
        {
            this._database = database;
        }

        public async Task Create(IReadOnlyCollection<SgCategoryAttributeMandatory> categoryAttributes)
        {
            if (categoryAttributes.Count <= 0)
            {
                return;
            }

            using var transaction = this._database.BeginTransactionOrUseExisting(out _);

            var tbl = AllTables.GetCategoryAttribute();

            await InsertDataInto(tbl, categoryAttributes)
                .MapData(s => SgCategoryAttributeMandatory.GetMapping(s).Set(tbl.Order, (s.Index + 1) * -1))
                .Exec(this._database);

            await this.NormalizeOrder(categoryAttributes.Select(i => i.CategoryId).Distinct().ToList());

            transaction.Commit();
        }

        public async Task Merge(IReadOnlyCollection<SgCategoryAttributeMandatory> categoryAttributes)
        {
            if (categoryAttributes.Count <= 0)
            {
                return;
            }

            using var transaction = this._database.BeginTransactionOrUseExisting(out _);

            var tbl = AllTables.GetCategoryAttribute();

            var allAttributes = categoryAttributes.Select(a => a.AttributeId).Distinct().ToList();

            var newOrder = CustomColumnFactory.Int32("NewOrder");

            await MergeDataInto(tbl, categoryAttributes)
                .MapDataKeys(SgCategoryAttributeMandatory.GetUpdateKeyMapping)
                .MapData(SgCategoryAttributeMandatory.GetUpdateMapping)
                .MapExtraData(m=>m.Set(newOrder, (m.Index+1)*-1))
                .WhenMatchedThenUpdate()
                .WhenNotMatchedByTargetThenInsert()
                .AlsoInsert(m=>m.Set(m.Target.Order, newOrder.WithSource(m.SourceDataAlias)))
                .WhenNotMatchedBySourceThenDelete(t => t.AttributeId.In(allAttributes))
                .Done()
                .Exec(this._database);

            var allCategories = categoryAttributes.Select(a => a.CategoryId).Distinct().ToList();
            await this.NormalizeOrder(allCategories);

            //Check orphan product attributes 

            var tblProduct = AllTables.GetProduct();
            var tblProductAttribute = AllTables.GetProductAttribute();
            var tblProductAttributeSet = AllTables.GetProductAttributeSet();
            var tblAttributeSet = AllTables.GetAttributeSet();

            await Delete(tblProductAttribute)
                .From(tblProductAttribute)
                .InnerJoin(tblProduct, on: tblProduct.ProductId == tblProductAttribute.ProductId)
                .Where(
                    tblProductAttribute.AttributeId.In(allAttributes) &
                    !Exists(
                        SelectOne()
                            .From(tbl)
                            .Where(
                                tbl.AttributeId == tblProductAttribute.AttributeId &
                                tbl.CategoryId == tblProduct.CategoryId)))
                .Exec(this._database);

            await Delete(tblProductAttributeSet)
                .From(tblProductAttributeSet)
                .InnerJoin(tblProduct, on: tblProduct.ProductId == tblProductAttributeSet.ProductId)
                .InnerJoin(tblAttributeSet, on: tblAttributeSet.AttributeSetId == tblProductAttributeSet.AttributeSetId)
                .Where(
                    tblAttributeSet.AttributeId.In(allAttributes) &
                    !Exists(
                        SelectOne()
                            .From(tbl)
                            .Where(
                                tbl.AttributeId == tblAttributeSet.AttributeId &
                                tbl.CategoryId == tblProduct.CategoryId)))
                .Exec(this._database);


            transaction.Commit();
        }

        public async Task Update(IReadOnlyCollection<SgCategoryAttributeOrder> categoryAttributes)
        {
            var tbl = AllTables.GetCategoryAttribute();
            await UpdateData(tbl, categoryAttributes)
                .MapDataKeys(SgCategoryAttributeOrder.GetUpdateKeyMapping)
                .MapData(SgCategoryAttributeOrder.GetUpdateMapping)
                .Exec(this._database);
        }

        private Task NormalizeOrder(IReadOnlyList<Guid> categories)
        {
            var subQuery = TableAlias();
            var tblSub = AllTables.GetCategoryAttribute();

            var numQuery = Select(
                    tblSub.CategoryId,
                    tblSub.AttributeId,
                    RowNumber()
                        .OverPartitionBy(tblSub.CategoryId)
                        .OverOrderBy(
                            Asc(Case().When(tblSub.Order < 0).Then(1).Else(0)),
                            Asc(Case().When(tblSub.Order < 0).Then(tblSub.Order * -1).Else(tblSub.Order)))
                        .As(tblSub.Order))
                .From(tblSub)
                .As(subQuery);

            var tbl = AllTables.GetCategoryAttribute();

            return SqQueryBuilder.Update(tbl)
                .Set(tbl.Order, tbl.Order.WithSource(subQuery))
                .From(tbl)
                .InnerJoin(numQuery,
                    on: tbl.CategoryId == tbl.CategoryId.WithSource(subQuery) &
                        tbl.AttributeId == tbl.AttributeId.WithSource(subQuery))
                .Where(tbl.CategoryId.In(categories))
                .Exec(this._database);
        }
    }
}