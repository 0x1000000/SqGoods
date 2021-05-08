using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqExpress;
using SqGoods.DomainLogic;
using SqGoods.DomainLogic.Models;
using SqGoods.Infrastructure;
using SqGoods.Models;
using SqGoods.Models.Fields;

namespace SqGoods.Services
{
    public class CategoriesService
    {
        private readonly IDomainLogic _domain;

        public CategoriesService(IDomainLogic domain)
        {
            this._domain = domain;
        }

        public async Task<DataPageModel<CategoryListModel>> Find(int offset, int pageSize)
        {
            return await SqModelSelectBuilder
                .Select(SgCategory.GetReader())
                .LeftJoin(
                    SgCategoryAttNum.GetReader(),
                    on: t => t.JoinedTable1.CategoryId == t.Table.CategoryId)
                .LeftJoin(
                    SgCategoryProductNum.GetReader(),
                    on: t => t.JoinedTable2.CategoryId == t.Table.CategoryId)
                .Find(offset,
                    pageSize,
                    null,
                    t => t.Table.Order,
                    d =>
                    {
                        var result = new CategoryListModel();
                        CopyToCategoryUpdateModel(result, d.Model);
                        result.AttributesCount = d.JoinedModel1?.AttributesCount ?? 0;
                        result.ProductCount = d.JoinedModel2?.ProductCount ?? 0;
                        return result;

                    })
                .QueryPage(this._domain.Db);
        }

        public async Task<IReadOnlyList<CategoryUpdateModel>> Get(IReadOnlyList<Guid> categoryId)
        {
            return await SqModelSelectBuilder
                .Select(SgCategory.GetReader())
                .Get(t=>t.CategoryId.In(categoryId), t => t.Order, r => CopyToCategoryUpdateModel(new CategoryUpdateModel(), r))
                .QueryList(this._domain.Db);
        }

        public async Task<IReadOnlyList<CategoryUpdateModel>> Create(IReadOnlyList<CategoryCreateModel> categories)
        {
            var newData = categories.Select(cm=> new SgCategory(Guid.NewGuid(), cm.Name, cm.Order, cm.TopOrder)).ToList();

            await this._domain.Category.Create(newData);

            return newData.Select(i => new CategoryUpdateModel {Id = i.Id, Name = i.Name}).ToList();
        }

        public async Task Update(IReadOnlyList<CategoryUpdateModel> categories)
        {
            var data = categories.Select(m => new SgCategory(m.Id, m.Name, m.Order, m.TopOrder)).ToList();

            await this._domain.Category.Update(SgCategory.GetUpdater(), data);
        }

        public async Task Delete(IReadOnlyList<Guid> categoryIds)
        {
            await this._domain.Category.Delete(categoryIds);
        }

        public async Task<IServiceResponse> AddCategories(Guid categoryId, IReadOnlyList<Guid> attributeIds)
        {
            if (attributeIds.Count < 1)
            {
                return ServiceResponse.Error(ServiceErrorCode.BadRequest, "Attributes list cannot be empty");
            }

            await this._domain.CategoryAttribute.Create(attributeIds
                .Select(aId => new SgCategoryAttributeMandatory(categoryId, aId, false))
                .ToList());

            return ServiceResponse.Successful();
        }

        public async Task<IReadOnlyList<FormFieldModel>> GetFormFields(Guid categoryId)
        {
            var attributes = await SqModelSelectBuilder
                .Select(SgAttribute.GetReader())
                .InnerJoin(SgCategoryAttributeMandatory.GetReader(), on: t => t.Table.AttributeId == t.JoinedTable1.AttributeId)
                .Get(t => t.JoinedTable1.CategoryId == categoryId, t => t.JoinedTable1.Order, n => (Attribute: n.Model, AttributeCtx: n.JoinedModel1))
                .QueryList(this._domain.Db);

            var selectAttributes = attributes
                .Where(a => a.Attribute.Type.IsSelectSet())
                .Select(a => a.Attribute.Id)
                .ToList();

            Dictionary<Guid, ICollection<SelectFieldItemModel>>? attItems = null;
            if (selectAttributes.Count > 0)
            {
                attItems = await SqModelSelectBuilder
                    .Select(SgAttributeItem.GetReader())
                    .Get(t => t.AttributeId.In(selectAttributes),
                        t => SqQueryBuilder.Asc(t.AttributeId).ThenBy(t.Order),
                        m => m)
                    .QueryDictGroup(this._domain.Db,
                        i => (i.AttributeId,
                            new SelectFieldItemModel { Id = i.AttributeSetId.ToString(), Title = i.Name }));
            }

            var result = new List<FormFieldModel>();
            foreach (var attribute in attributes)
            {
                var factory = new FieldDescriptorFactory(attribute.Attribute, attItems);
                var descriptor = attribute.Attribute.Type.Switch<FieldDescriptorFactory, FieldDescriptorModel>(ref factory);

                result.Add(new FormFieldModel
                {
                    Descriptor = descriptor,
                    Mandatory = attribute.AttributeCtx.Mandatory
                });
            }
            return result;
        }

        private readonly struct FieldDescriptorFactory : ISgAttributeTypeVisitor<FieldDescriptorModel>
        {
            private readonly SgAttribute _attribute;

            private readonly IReadOnlyDictionary<Guid, ICollection<SelectFieldItemModel>>? _itemsDict;

            public FieldDescriptorFactory(SgAttribute attribute, IReadOnlyDictionary<Guid, ICollection<SelectFieldItemModel>>? itemsDict)
            {
                this._attribute = attribute;
                this._itemsDict = itemsDict;
            }

            public FieldDescriptorModel CaseBoolean()
            {
                var result = new BooleanFieldDescriptorModel();
                this.FillCommon(result);
                return result;
            }

            public FieldDescriptorModel CaseInteger()
            {
                var result = new NumberFieldDescriptorModel();
                this.FillCommon(result);
                return result;
            }

            public FieldDescriptorModel CaseSelect()
            {
                var result = new SelectFieldDescriptorModel();
                this.FillCommon(result);
                result.Items = this.GetItems();
                return result;
            }

            public FieldDescriptorModel CaseSubset()
            {
                var result = new SelectFieldDescriptorModel();
                this.FillCommon(result);
                result.Items = this.GetItems();
                result.Multi = true;
                return result;
            }

            private void FillCommon(FieldDescriptorModel fieldDescriptorModel)
            {
                fieldDescriptorModel.Id = this._attribute.Id.ToString();
                fieldDescriptorModel.Label = this._attribute.Name;
                fieldDescriptorModel.Note = this._attribute.Unit;
            }

            private List<SelectFieldItemModel> GetItems()
            {
                if (this._itemsDict == null || !this._itemsDict.TryGetValue(this._attribute.Id, out var list))
                {
                    return new List<SelectFieldItemModel>(0);
                }

                return list as List<SelectFieldItemModel> ?? list.ToList();
            }
        }

        private static CategoryUpdateModel CopyToCategoryUpdateModel(CategoryUpdateModel updateModel, SgCategory sgCategoryName)
        {
            updateModel.Id = sgCategoryName.Id;
            updateModel.Name = sgCategoryName.Name;
            updateModel.Order = sgCategoryName.Order;
            updateModel.TopOrder = sgCategoryName.TopOrder;
            return updateModel;
        }
    }
}