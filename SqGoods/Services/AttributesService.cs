using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.Syntax.Boolean;
using SqGoods.DomainLogic;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Tables;
using SqGoods.Infrastructure;
using SqGoods.Models;
using static SqExpress.SqQueryBuilder;

namespace SqGoods.Services
{
    public class AttributesService
    {
        private readonly IDomainLogic _domain;

        public AttributesService(IDomainLogic domain)
        {
            this._domain = domain;
        }

        public async Task<AttributeMetaListModel> Find(int offset, int pageSize, Guid? categoryId, bool inverseCategoryId)
        {
            DataPage<AttributeListModel> page;

            if (categoryId.HasValue && !inverseCategoryId)
            {
                page = await SqModelSelectBuilder
                    .Select(SgAttribute.GetReader())
                    .InnerJoin(SgCategoryAttributeMandatory.GetReader(),
                        on: t => t.JoinedTable1.AttributeId == t.Table.AttributeId)
                    .LeftJoin(SgAttributeCatsNum.GetReader(),
                        on: t => t.Table.AttributeId == t.JoinedTable2.AttributeId)
                    .LeftJoin(SgAttributeSetItemsNum.GetReader(),
                        on: t => t.Table.AttributeId == t.JoinedTable3.AttributeId)
                    .Find(offset,
                        pageSize,
                        t => t.JoinedTable1.CategoryId == categoryId.Value,
                        t => Asc(t.JoinedTable1.Order),
                        i => new AttributeListModel
                        {
                            Id = i.Model.Id,
                            Name = i.Model.Name,
                            Type = i.Model.Type,
                            Unit = i.Model.Unit,
                            NumberOfCategories = i.JoinedModel2?.CategoriesCount ?? 0,
                            NumberOfItems = i.JoinedModel3?.SetItemsNum,
                            Mandatory = i.JoinedModel1.Mandatory
                        })
                    .QueryPage(this._domain.Db);
            }
            else
            {
                Func<TblAttribute,ExprBoolean>? filter = null;
                if (categoryId.HasValue && inverseCategoryId)
                {
                    var catAtt = new TblCategoryAttribute();
                    filter = t =>
                        !Exists(SelectOne()
                            .From(catAtt)
                            .Where(catAtt.CategoryId == categoryId & catAtt.AttributeId == t.AttributeId));
                }

                page = await SqModelSelectBuilder
                    .Select(SgAttribute.GetReader())
                    .LeftJoin(SgAttributeCatsNum.GetReader(), on: t => t.Table.AttributeId == t.JoinedTable1.AttributeId)
                    .LeftJoin(SgAttributeSetItemsNum.GetReader(), on: t => t.Table.AttributeId == t.JoinedTable2.AttributeId)
                    .Find(offset,
                        pageSize,
                        filter != null ? t=> filter(t.Table): null,
                        t => t.Table.Name,
                        i => new AttributeListModel
                        {
                            Id = i.Model.Id,
                            Name = i.Model.Name,
                            Type = i.Model.Type,
                            Unit = i.Model.Unit,
                            NumberOfCategories = i.JoinedModel1?.CategoriesCount ?? 0,
                            NumberOfItems = i.JoinedModel2?.SetItemsNum
                        })
                    .QueryPage(this._domain.Db);
            }

            var cats = await SqModelSelectBuilder
                .Select(SgCategoryName.GetReader())
                .LeftJoin(SgCategoryAttNum.GetReader(), on: t => t.Table.CategoryId == t.JoinedTable1.CategoryId)
                .Get(null, t => t.Table.Order, n => new CategoryListModel
                {
                    Id = n.Model.Id,
                    Name = n.Model.Name,
                    AttributesCount = n.JoinedModel1?.AttributesCount ?? 0
                })
                .QueryList(this._domain.Db);

            return new AttributeMetaListModel(page.Items, page.Offset, page.Total, cats);
        }

        public async Task<IReadOnlyList<AttributeUpdateModel>> Get(IReadOnlyList<Guid> attributeId)
        {
            var attributes = await SqModelSelectBuilder
                .Select(SgAttribute.GetReader())
                .Get(t => t.AttributeId.In(attributeId), null, i => i)
                .QueryDict(this._domain.Db, i=> (i.Id, i));

            var categories = await SqModelSelectBuilder
                .Select(SgCategoryAttributeMandatory.GetReader())
                .Get(t => t.AttributeId.In(attributeId), t => t.Order, i => i)
                .QueryDictGroup(this._domain.Db, i=>(i.AttributeId, i));

            return attributeId.SelectReadOnlyList(id =>
                {
                    var attribute = attributes[id];
                    var attCategories = categories.TryGetValue(id, out var val) ? (IReadOnlyList<SgCategoryAttributeMandatory>?)val : null;

                    return new AttributeUpdateModel
                    {
                        Id = id,
                        Name = attribute.Name,
                        Type = attribute.Type,
                        Categories = attCategories?.Select(i=>i.CategoryId).ToList(),
                        Unit = attribute.Unit,
                        Mandatory = attCategories?.All(ac=>ac.Mandatory) ?? false
                    };
                });
        }

        public async Task Create(IReadOnlyList<AttributeCreateModel> createModel)
        {
            var (tran, _) = await this._domain.Db.BeginTransactionOrUseExistingAsync();
            await using (tran)
            {

                var attrs = createModel.SelectReadOnlyList(i => new SgAttribute(Guid.NewGuid(), i.Name, i.Type, i.Unit));

                await this._domain.Attribute.Create(attrs);

                var attrCat = attrs
                    .Zip(createModel, (attr, cModel) => (attr, cModel))
                    .SelectMany(
                        t => t.cModel.Categories!.Select(
                            cat => new SgCategoryAttributeMandatory(cat, t.attr.Id, t.cModel.Mandatory)
                        )
                    )
                    .ToList();

                await this._domain.CategoryAttribute.Create(attrCat);

                await tran.CommitAsync();
            }
        }

        public async Task Update(IReadOnlyList<AttributeUpdateModel> createModel)
        {
            var (tran, _) = await this._domain.Db.BeginTransactionOrUseExistingAsync();
            await using (tran)
            {

                var attrs = createModel.SelectReadOnlyList(i => new SgAttribute(i.Id, i.Name, i.Type, i.Unit));

                await this._domain.Attribute.Update(attrs);

                var attrCat = attrs
                    .Zip(createModel, (attr, cModel) => (attr, cModel))
                    .SelectMany(
                        t => t.cModel.Categories!.Select(
                            cat => new SgCategoryAttributeMandatory(cat, t.attr.Id, t.cModel.Mandatory)
                        )
                    )
                    .ToList();

                await this._domain.CategoryAttribute.Merge(attrCat);

                await tran.CommitAsync();
            }
        }

        public async Task Delete(IReadOnlyList<Guid> attributes)
        {
            await this._domain.Attribute.Delete(attributes);
        }

        public async Task<IServiceResponse<IReadOnlyList<AttributeItemModel>>> GetItems(Guid attributeId)
        {
            var error = await this.CheckSetAttribute(attributeId: attributeId);
            if (error != null)
            {
                return ServiceResponse.Error<IReadOnlyList<AttributeItemModel>>(error);
            }

            var page = await SqModelSelectBuilder
                .Select(SgAttributeItemName.GetReader())
                .Find(0, 1000, t => t.AttributeId == attributeId, t => t.Order, i => new AttributeItemModel{Id = i.AttributeSetId, Title = i.Name })
                .QueryPage(this._domain.Db);

            return ServiceResponse.Successful(page.Items);
        }

        public async Task<IServiceResponse> UpdateItems(Guid attributeId, IReadOnlyList<AttributeItemModel> items)
        {
            var (tran, _) = await this._domain.Db.BeginTransactionOrUseExistingAsync();
            await using (tran)
            {
                var error = await this.CheckSetAttribute(attributeId: attributeId);
                if (error != null)
                {
                    return ServiceResponse.Error(error);
                }

                if (items.Count < 1)
                {
                    await this._domain
                        .AttributeSet
                        .Delete(new[] { attributeId });
                }
                else
                {
                    await this._domain
                        .AttributeSet
                        .Merge(
                            items.Select((i, index) => new SgAttributeItem(i.Id ?? Guid.NewGuid(), attributeId, i.Title, index))
                                .ToList()
                        );
                }

                await tran.CommitAsync();
            }

            return ServiceResponse.Successful();
        }

        private async Task<ServiceErrorData?> CheckSetAttribute(Guid attributeId)
        {
            var attribute =
                (await SqModelSelectBuilder.Select(SgAttribute.GetReader())
                    .Get(t => t.AttributeId == attributeId, null, i => i)
                    .QueryList(this._domain.Db)).SingleOrDefault();

            if (attribute == null)
            {
                return ServiceResponse.ErrorData(ServiceErrorCode.NotFound, null);
            }

            if (!attribute.Type.IsSelectSet())
            {
                return ServiceResponse.ErrorData(ServiceErrorCode.BadRequest, "Incorrect attribute type");
            }

            return null;
        }

        public async Task<IServiceResponse> SwapAttributes(IReadOnlyList<AttributeSwapFormModel> forms)
        {
            if (forms.Count < 1)
            {
                return ServiceResponse.Error(ServiceErrorCode.BadRequest, "At least one form is expected");
            }

            var filterData = new List<(Guid CategoryId, IReadOnlyList<Guid> AttributeIds)>(forms.Count);
            foreach (var form in forms)
            {
                var allAttributes = form.Swaps.SelectMany(f => new[] { f.CurrentId, f.OriginalId }).Distinct().ToList();
                if (allAttributes.Count < 1)
                {
                    return ServiceResponse.Error(ServiceErrorCode.BadRequest, "There should be at least one attribute");
                }
                if (allAttributes.Count != form.Swaps.Count)
                {
                    return ServiceResponse.Error(ServiceErrorCode.BadRequest, "Attributes should be different and unique");
                }
                filterData.Add((form.CategoryId, allAttributes));
            }

            var order = await SqModelSelectBuilder.Select(SgCategoryAttributeOrder.GetReader())
                .Get(t => filterData.Select(f => t.CategoryId == f.CategoryId & t.AttributeId.In(f.AttributeIds)).JoinAsOr(), null, i=>i)
                .QueryDict(this._domain.Db, i=>((i.CategoryId, i.AttributeId), i.Order));


            var newData = new List<SgCategoryAttributeOrder>();

            foreach (var form in forms)
            {
                foreach (var swap in form.Swaps)
                {
                    if (!order.TryGetValue((form.CategoryId, swap.OriginalId), out var originalOrder))
                    {
                        return ServiceResponse.Error(ServiceErrorCode.NotFound, $"Could not find category {form.CategoryId} attribute {swap.OriginalId}");
                    }
                    newData.Add(new SgCategoryAttributeOrder(form.CategoryId, swap.CurrentId, originalOrder ));
                }

            }

            await this._domain.CategoryAttribute.Update(newData);

            return ServiceResponse.Successful();
        }
    }
}