using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Repositories.Utils;
using SqGoods.DomainLogic.Tables;
using static SqExpress.SqQueryBuilder;

namespace SqGoods.DomainLogic.Repositories
{
    public interface ISgAttributeRepository
    {
        Task Create(IReadOnlyList<SgAttribute> attributes);

        Task Update(IReadOnlyList<SgAttribute> attributes);

        public interface IRelationFields
        {
            RelationField CategoryId { get; }
        }

        Task Delete(IReadOnlyList<Guid> attributes);
    }


    internal class SgAttributeRepository : ISgAttributeRepository
    {
        private readonly ISqDatabase _database;

        private static readonly RelationFields RelationFieldsInstance = new RelationFields();

        public SgAttributeRepository(ISqDatabase database)
        {
            this._database = database;
        }

        public async Task Create(IReadOnlyList<SgAttribute> attributes)
        {
            var tbl = AllTables.GetAttribute();

            await InsertDataInto(tbl, attributes).MapData(SgAttribute.GetMapping).Exec(this._database);
        }

        public async Task Update(IReadOnlyList<SgAttribute> attributes)
        {
            var tbl = AllTables.GetAttribute();

            var (tran, _) = await this._database.BeginTransactionOrUseExistingAsync();
            await using (tran)
            {


                var attChangedType = await SqModelSelectBuilder.Select(SgAttributeTypeId.GetReader())
                    .Get(
                        t => attributes.Select(a => a.Id == t.AttributeId & (int)a.Type != t.Type).JoinAsOr(),
                        null,
                        i => i
                    )
                    .Query(
                        this._database,
                        (SingleValue: new List<Guid>(), Set: new List<Guid>()),
                        (acc, next) =>
                        {
                            (next.Type == SgAttributeType.SubSet ? acc.Set : acc.SingleValue).Add(next.Id);
                            return acc;
                        }
                    );

                await UpdateData(tbl, attributes)
                    .MapDataKeys(SgAttribute.GetUpdateKeyMapping)
                    .MapData(SgAttribute.GetUpdateMapping)
                    .Exec(this._database);

                //Removing product values for attributes whose type have been changed
                if (attChangedType.SingleValue.Count > 0)
                {
                    var tblProductAttribute = AllTables.GetProductAttribute();
                    await SqQueryBuilder.Delete(tblProductAttribute)
                        .Where(tblProductAttribute.AttributeId.In(attChangedType.SingleValue))
                        .Exec(this._database);
                }

                if (attChangedType.Set.Count > 0)
                {
                    var tblProductAttributeSet = AllTables.GetProductAttributeSet();
                    var tblAttSet = AllTables.GetAttributeSet();

                    await SqQueryBuilder.Delete(tblProductAttributeSet)
                        .From(tblProductAttributeSet)
                        .InnerJoin(tblAttSet, on: tblProductAttributeSet.AttributeSetId == tblAttSet.AttributeSetId)
                        .Where(tblAttSet.AttributeId.In(attChangedType.Set))
                        .Exec(this._database);
                }

                await tran.CommitAsync();
            }
        }

        public async Task Delete(IReadOnlyList<Guid> attributes)
        {
            var tbl = AllTables.GetAttribute();
            var tblSet = AllTables.GetAttributeSet();
            var tblCat = AllTables.GetCategoryAttribute();

            var tblProductAttribute = AllTables.GetProductAttribute();
            var tblProductAttributeSet = AllTables.GetProductAttributeSet();

            var (tran, _) = await this._database.BeginTransactionOrUseExistingAsync();
            await using (tran)
            {
                await SqQueryBuilder
                    .Delete(tblProductAttribute)
                    .Where(tblProductAttribute.AttributeId.In(attributes))
                    .Exec(this._database);

                await SqQueryBuilder
                    .Delete(tblProductAttributeSet)
                    .From(tblProductAttributeSet)
                    .InnerJoin(tblSet, on: tblProductAttributeSet.AttributeSetId == tblSet.AttributeSetId)
                    .Where(tblSet.AttributeId.In(attributes))
                    .Exec(this._database);

                await SqQueryBuilder
                    .Delete(tblSet)
                    .Where(tblSet.AttributeId.In(attributes))
                    .Exec(this._database);

                await SqQueryBuilder
                    .Delete(tblCat)
                    .Where(tblCat.AttributeId.In(attributes))
                    .Exec(this._database);

                await SqQueryBuilder
                    .Delete(tbl)
                    .Where(tbl.AttributeId.In(attributes))
                    .Exec(this._database);

                await tran.CommitAsync();
            }
        }

        private class RelationFields : RelationFieldsBase<TblAttribute>, ISgAttributeRepository.IRelationFields
        {
            public RelationField CategoryId { get; }

            public RelationFields()
            {
                this.CategoryId = Create("CategoryId",
                    (t, items) =>
                    {
                        var tblCategoryAttribute = AllTables.GetCategoryAttribute();

                        return Exists(SelectOne()
                            .From(tblCategoryAttribute)
                            .Where(tblCategoryAttribute.AttributeId == t.AttributeId &
                                   tblCategoryAttribute.CategoryId.In(items)));
                    });
            }
        }
    }
}