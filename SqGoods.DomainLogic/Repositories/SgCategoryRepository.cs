using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqExpress.Syntax.Boolean.Predicate;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Repositories.Utils;
using SqGoods.DomainLogic.Tables;
using static SqExpress.SqQueryBuilder;

namespace SqGoods.DomainLogic.Repositories
{
    public interface ISgCategoryRepository
    {
        Task Create(IReadOnlyList<SgCategory> newCategories);

        Task Update<T>(ISqModelUpdaterKey<T, TblCategory> updater, IReadOnlyList<T> data) where T : ISgCategoryIdentity;

        Task Delete(IReadOnlyList<Guid> categoriesIds);
    }

    internal class SgCategoryRepository : ISgCategoryRepository
    {
        private readonly ISqDatabase _database;

        public SgCategoryRepository(ISqDatabase database)
        {
            this._database = database;
        }

        public async Task Create(IReadOnlyList<SgCategory> newCategories)
        {
            using var transaction = this._database.BeginTransactionOrUseExisting(out _);

            var tblCategory =  AllTables.GetCategory();

            await InsertDataInto(tblCategory, newCategories)
                .MapData(SgCategory.GetMapping)
                .Exec(this._database);

            await this.UpdateOrder(newCategories, null);

            transaction.Commit();
        }

        public async Task Update<T>(ISqModelUpdaterKey<T, TblCategory> updater, IReadOnlyList<T> data)
            where T : ISgCategoryIdentity
        {
            using var transaction = this._database.BeginTransactionOrUseExisting(out _);

            TmpOrder? tmpOrder = null;

            if (typeof(ISgCategoryOrder).IsAssignableFrom(typeof(T)))
            {
                tmpOrder = new TmpOrder();

                var cat = AllTables.GetCategory();

                await this._database.Statement(tmpOrder.Script.Create());

                await InsertInto(tmpOrder, tmpOrder.CategoryId, tmpOrder.OldOrder, tmpOrder.OldTopOrder)
                    .From(Select(cat.CategoryId, cat.Order, cat.TopOrder)
                        .From(cat)
                        .Where(cat.CategoryId.In(data.Select(i => i.Id).ToList())))
                    .Exec(this._database);
            }

            await QueryHelper.Update(this._database, data, updater, s=>s.Set(s.Target.DateTimeUpdated, GetUtcDate()));

            if (tmpOrder != null)
            {
                await UpdateOrder((IReadOnlyList<ISgCategoryOrder>) data, tmpOrder);

                await this._database.Statement(tmpOrder.Script.Drop());
            }

            transaction.Commit();
        }

        public async Task Delete(IReadOnlyList<Guid> categoriesIds)
        {
            var tbl = AllTables.GetCategory();
            var tblCatAttr = AllTables.GetCategoryAttribute();
            var tblProduct = AllTables.GetProduct();
            var tblProductAttribute = AllTables.GetProductAttribute();
            var tblProductAttributeSet = AllTables.GetProductAttributeSet();

            using var tran = this._database.BeginTransactionOrUseExisting(out _);

            await SqQueryBuilder
                .Delete(tblProductAttributeSet)
                .From(tblProductAttributeSet)
                .InnerJoin(tblProduct, on: tblProduct.ProductId == tblProductAttributeSet.ProductId)
                .Where(tblProduct.CategoryId.In(categoriesIds))
                .Exec(this._database);

            await SqQueryBuilder
                .Delete(tblProductAttribute)
                .From(tblProductAttribute)
                .InnerJoin(tblProduct, on: tblProduct.ProductId == tblProductAttribute.ProductId)
                .Where(tblProduct.CategoryId.In(categoriesIds))
                .Exec(this._database);

            await SqQueryBuilder
                .Delete(tblProduct)
                .Where(tblProduct.CategoryId.In(categoriesIds))
                .Exec(this._database);

            await SqQueryBuilder
                .Delete(tblCatAttr)
                .Where(tblCatAttr.CategoryId.In(categoriesIds))
                .Exec(this._database);

            await SqQueryBuilder
                .Delete(tbl)
                .Where(tbl.CategoryId.In(categoriesIds))
                .Exec(this._database);

            await this.UpdateOrder(null, null);

            tran.Commit();
        }

        private async Task UpdateOrder(IReadOnlyList<ISgCategoryOrder>? newCategories, TmpOrder? oldOrder)
        {
            TblCategory tblCategory = AllTables.GetCategory();

            //Order updating
            var tblSubCategory = AllTables.GetCategory();
            var subQuery = TableAlias();
            var colNewOrder = CustomColumnFactory.Int32("NewOrder");

            var exprOrderByItem = oldOrder == null 
                ? Desc(tblSubCategory.DateTimeUpdated)
                : Asc(Case().When(IsNotNull(oldOrder.OldOrder) & new ExprBooleanGt(oldOrder.OldOrder, tblSubCategory.Order)).Then(0).Else(1));

            var subSelect = Select(
                    tblSubCategory.CategoryId,
                    RowNumber()
                        .OverOrderBy(Asc(tblSubCategory.Order), exprOrderByItem)
                        .As(colNewOrder))
                .From(tblSubCategory);

            if (oldOrder != null)
            {
                subSelect = subSelect.LeftJoin(oldOrder, on: oldOrder.CategoryId == tblSubCategory.CategoryId);
            }

            await SqQueryBuilder.Update(tblCategory)
                .Set(tblCategory.Order, subQuery.Column(colNewOrder))
                .From(subSelect.As(subQuery))
                .InnerJoin(tblCategory, on: tblCategory.CategoryId == subQuery.Column(tblSubCategory.CategoryId))
                .Where(tblCategory.Order != subQuery.Column(colNewOrder))
                .Exec(this._database);

            //Top order updating
            if (newCategories == null || newCategories.Any(c => c.TopOrder.HasValue) || oldOrder != null)
            {
                exprOrderByItem = oldOrder == null
                    ? Desc(tblSubCategory.DateTimeUpdated)
                    : Asc(Case().When(IsNotNull(oldOrder.OldTopOrder) & new ExprBooleanGt(oldOrder.OldTopOrder, tblSubCategory.TopOrder)).Then(0).Else(1));

                subSelect = Select(
                        tblSubCategory.CategoryId,
                        RowNumber()
                            .OverOrderBy(Asc(tblSubCategory.TopOrder), exprOrderByItem)
                            .As(colNewOrder))
                    .From(tblSubCategory);

                if (oldOrder != null)
                {
                    subSelect = subSelect.LeftJoin(oldOrder, on: oldOrder.CategoryId == tblSubCategory.CategoryId);
                }

                await SqQueryBuilder.Update(tblCategory)
                    .Set(tblCategory.TopOrder, subQuery.Column(colNewOrder))
                    .From(subSelect.Where(IsNotNull(tblSubCategory.TopOrder)).As(subQuery))
                    .InnerJoin(tblCategory, on: tblCategory.CategoryId == subQuery.Column(tblSubCategory.CategoryId))
                    .Where(tblCategory.TopOrder != subQuery.Column(colNewOrder))
                    .Exec(this._database);
            }
        }

        private class TmpOrder : TempTableBase
        {
            public TmpOrder() : base("tmpPreviousOrder")
            {
                this.CategoryId =  this.CreateGuidColumn("CategoryId", ColumnMeta.PrimaryKey());
                this.OldOrder =  this.CreateInt32Column("OldOrder");
                this.OldTopOrder =  this.CreateNullableInt32Column("OldTopOrder");
            }

            public GuidTableColumn CategoryId { get; }

            public Int32TableColumn OldOrder { get;  }

            public NullableInt32TableColumn OldTopOrder { get; }
        }
    }
}