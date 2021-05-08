using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqExpress.Syntax.Boolean;
using SqGoods.DomainLogic;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Tables;
using SqGoods.Infrastructure;
using SqGoods.Models;
using SqGoods.Models.Fields;
using SqGoods.Models.Filter;

namespace SqGoods.Services
{
    public class ProductService
    {
        private readonly IDomainLogic _domain;

        public ProductService(IDomainLogic domain)
        {
            this._domain = domain;
        }

        public async Task<IReadOnlyList<IdNameModel>> GetCategories()
        {
            var cats = await SqModelSelectBuilder
                .Select(SgCategoryName.GetReader())
                .Get(null, t => t.Order, n => new IdNameModel
                {
                    Id = n.Id,
                    Name = n.Name
                })
                .QueryList(this._domain.Db);

            return cats;
        }

        public async Task<IServiceResponse<DataPageModel<ProductListModel>>> Find(int offset, int pageSize, Guid? categoryId, FilterBoolModel? extraFilterModel)
        {
            ExprBoolean? extraFilter = null;
            if (extraFilterModel != null)
            {
                string? error = null;
                (extraFilter, error) = extraFilterModel.Accept(FilterBoolModelMapper.Instance);
                if (error != null)
                {
                    return ServiceResponse.Error<DataPageModel<ProductListModel>>(ServiceErrorCode.BadRequest, error);
                }
            }

            var productPage = await this._domain.Product.Find(offset, pageSize, categoryId, extraFilter);

            if (productPage.Items.Count < 1)
            {
                return ServiceResponse.Successful(new DataPageModel<ProductListModel>(Array.Empty<ProductListModel>(), 0, 0));
            }

            var categoryIds = productPage.Items.Select(p => p.CategoryId).Distinct().ToList();
            var productIds = productPage.Items.Select(p => p.ProductId).ToList();

            var attByCatLists = await GetAttributesByCategories(this._domain.Db, categoryIds);

            var attributesIds = attByCatLists.SelectMany(c => c.Value).Distinct().ToList();

            var attributes = await GetAttributesByIds(this._domain.Db, attributesIds);
            var productAttributeRawValues = await GetProductAttributeRawValues(this._domain.Db, attributesIds, productIds);
            var productAttributeSetValues = await GetProductAttributeSetValues(this._domain.Db, attributesIds, productIds);

            var selectEnum = productAttributeRawValues
                .Where(pv => attributes[pv.Key.AttributeId].Type == SgAttributeType.Select)
                .Select(pv => pv.Value.SelectValue ?? throw new Exception("Select value is expected here"));
            var allSetIds = productAttributeSetValues.Values.SelectMany(i=>i).Concat(selectEnum).Distinct().ToList();
            var setItemNames = allSetIds.Count < 1
                ? new Dictionary<Guid, string>(0)
                : await SqModelSelectBuilder.Select(SgAttributeItemName.GetReader())
                    .Get(t => t.AttributeSetId.In(allSetIds), null, i => i)
                    .QueryDict(this._domain.Db, i => (i.AttributeSetId, i.Name));

            var resultItems = new List<ProductListModel>(productPage.Items.Count);
            foreach (var product in productPage.Items)
            {
                var catAttributes = attByCatLists[product.CategoryId];
                var attributeModels = new List<ProductListAttributeModel>(catAttributes.Count);
                foreach (var attributeId in catAttributes)
                {
                    var attribute = attributes[attributeId];
                    var strValue = attribute.Type != SgAttributeType.SubSet
                        ? productAttributeRawValues.TryGetValue((product.ProductId, attributeId), out var valueRow)
                            ? PrintDisplay(attribute.Type, valueRow, setItemNames)
                            : "-"
                        : productAttributeSetValues.TryGetValue((product.ProductId, attributeId), out var itemIds)
                            ? string.Join(", ", itemIds.Select(id => setItemNames[id]))
                            : "-";

                    if (!string.IsNullOrEmpty(attribute.Unit))
                    {
                        strValue += " " + attribute.Unit;
                    }
                    attributeModels.Add(new ProductListAttributeModel
                    {
                        Name = attribute.Name,
                        Value = strValue
                    });
                }

                resultItems.Add(new ProductListModel
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    CoverUrl = product.CoverUrl,
                    Attributes = attributeModels
                });
            }

            return ServiceResponse.Successful(new DataPageModel<ProductListModel>(resultItems, productPage.Offset, productPage.Total));

            //Local Functions
            static async Task<Dictionary<Guid, ICollection<Guid>>> GetAttributesByCategories(ISqDatabase database, IReadOnlyList<Guid> categoryIds)
            {
                return await SqModelSelectBuilder.Select(SgCategoryAttributeMandatory.GetReader())
                    .Get(t => t.CategoryId.In(categoryIds), t => t.Order, i => i)
                    .Query(database,
                        new Dictionary<Guid, ICollection<Guid>>(),
                        (acc, next) => acc.AppendGroup(next.CategoryId, next.AttributeId));
            }

            static async Task<Dictionary<Guid, SgAttribute>> GetAttributesByIds(ISqDatabase database, IReadOnlyList<Guid> attributesIds)
            {
                return attributesIds.Count < 1
                    ? new Dictionary<Guid, SgAttribute>(0)
                    : await SqModelSelectBuilder.Select(SgAttribute.GetReader())
                        .Get(i => i.AttributeId.In(attributesIds), null, i => i)
                        .QueryDict(database, i => (i.Id, i));
            }

            async Task<Dictionary<(Guid ProductId, Guid AttributeId), SgProductAttribute>> GetProductAttributeRawValues(ISqDatabase database, IReadOnlyList<Guid> attributesIds, List<Guid> productIds)
            {
                return attributesIds.Count < 1 || productIds.Count < 1
                    ? new Dictionary<(Guid ProductId, Guid AttributeId), SgProductAttribute>(0)
                    : await SqModelSelectBuilder
                        .Select(SgProductAttribute.GetReader())
                        .Get(t => t.ProductId.In(productIds) & t.AttributeId.In(attributesIds), null, i => i)
                        .QueryDict(database, i => ((i.ProductId, i.AttributeId), i));
            }

            async Task<Dictionary<(Guid ProductId, Guid AttributeId), ICollection<Guid>>> GetProductAttributeSetValues(ISqDatabase database, IReadOnlyList<Guid> attributesIds, IReadOnlyList<Guid> productIds)
            {
                return attributesIds.Count < 1 || productIds.Count < 1
                    ? new Dictionary<(Guid ProductId, Guid AttributeId), ICollection<Guid>>(0)
                    : await SqModelSelectBuilder
                        .Select(SgProductAttributeSet.GetReader())
                        .InnerJoin(SgAttributeSetId.GetReader(),
                            on: t => t.Table.AttributeSetId == t.JoinedTable1.AttributeSetId)
                        .Get(filter: t => t.Table.ProductId.In(productIds),
                            order: t => t.JoinedTable1.Order,
                            mapper: i => (i.Model.ProductId, i.JoinedModel1.AttributeId, i.JoinedModel1.AttributeSetId))
                        .QueryDictGroup(database, i => ((i.ProductId, i.AttributeId), i.AttributeSetId));
            }
        }

        public async Task<IServiceResponse<IReadOnlyList<ProductUpdateModel>>> Get(IReadOnlyList<Guid> productIds)
        {
            if (productIds.Count < 1)
            {
                return ServiceResponse.Successful<IReadOnlyList<ProductUpdateModel>>(Array.Empty<ProductUpdateModel>());
            }

            var products = await SqModelSelectBuilder
                .Select(SgProduct.GetReader())
                .Get(filter: t => t.ProductId.In(productIds), order: t => t.Name, mapper: i => i)
                .QueryDict(this._domain.Db, i=>(i.ProductId, i));

            var attributeTypes = (IReadOnlyDictionary<Guid, SgAttributeType>)await SqModelSelectBuilder
                .Select(SgAttributeTypeId.GetReader())
                .Get(filter: t => SqQueryBuilder.ExistsIn<TblProductAttribute>(
                        tPa => tPa.AttributeId == t.AttributeId & tPa.ProductId.In(productIds)), 
                     order: null, 
                     mapper:i => i)
                .QueryDict(this._domain.Db, i => (i.Id, i.Type));

            var productAttributeValues = await SqModelSelectBuilder
                .Select(SgProductAttribute.GetReader())
                .Get(t => t.ProductId.In(productIds), null, i => i)
                .QueryDictGroup(this._domain.Db, i => (i.ProductId, i));

            var productAttributeSetValues = await GetProductAttributeSetValuesForProductIds(this._domain.Db, productIds);

            var result = new List<ProductUpdateModel>();

            foreach (var productId in productIds)
            {
                if (!products.TryGetValue(productId, out var product))
                {
                    return ServiceResponse.Error<IReadOnlyList<ProductUpdateModel>>(
                        ServiceErrorCode.NotFound,
                        $"Could not find product by id: {product}");
                }

                result.Add(new ProductUpdateModel
                {
                    Id = product.ProductId,
                    CategoryId = product.CategoryId,
                    Name = product.Name,
                    ImageUrl = product.CoverUrl,
                    Values = GetAttributeValueModelsForProduct(productId)
                });
            }

            return ServiceResponse.Successful<IReadOnlyList<ProductUpdateModel>>(result);

            //Local functions
            List<AttributeValueModel> GetAttributeValueModelsForProduct(Guid productId)
            {
                var valueModels = new List<AttributeValueModel>();

                if (productAttributeValues.TryGetValue(productId, out var attributeValues))
                {
                    foreach (var productAttributeValue in attributeValues)
                    {
                        valueModels.Add(new AttributeValueModel
                        {
                            AttributeId = productAttributeValue.AttributeId,
                            Value = Print(attributeTypes[productAttributeValue.AttributeId], productAttributeValue)
                        });
                    }
                }
                if (productAttributeSetValues.TryGetValue(productId, out var attSets))
                {
                    foreach (KeyValuePair<Guid, ICollection<Guid>> attSet in attSets)
                    {
                        valueModels.Add(new AttributeValueModel { AttributeId = attSet.Key, Value = string.Join(';', attSet.Value) });
                    }
                }

                return valueModels;
            }

            static async Task<IReadOnlyDictionary<Guid, Dictionary<Guid, ICollection<Guid>>>> GetProductAttributeSetValuesForProductIds(ISqDatabase database, IReadOnlyList<Guid> productIds)
            {
                return await SqModelSelectBuilder
                    .Select(SgProductAttributeSet.GetReader())
                    .InnerJoin(SgAttributeSetId.GetReader(),
                        on: t => t.Table.AttributeSetId == t.JoinedTable1.AttributeSetId)
                    .Get(t => t.Table.ProductId.In(productIds),
                        null,
                        i => (i.Model.ProductId, i.JoinedModel1.AttributeId, i.Model.AttributeSetId))
                    .Query(database,
                        new Dictionary<Guid, Dictionary<Guid, ICollection<Guid>>>(),
                        (acc, next) =>
                        {
                            if (!acc.TryGetValue(next.ProductId, out var attDict))
                            {
                                attDict = new Dictionary<Guid, ICollection<Guid>>();
                                acc.Add(next.ProductId, attDict);
                            }

                            if (!attDict.TryGetValue(next.AttributeId, out var setList))
                            {
                                setList = new List<Guid>();

                                attDict.Add(next.AttributeId, setList);
                            }

                            setList.Add(next.AttributeSetId);

                            return acc;
                        });
            }
        }

        public async Task<IServiceResponse> Update(IReadOnlyList<ProductUpdateModel> updateModels)
        {
            if (updateModels.Count < 1)
            {
                return ServiceResponse.Error(ServiceErrorCode.BadRequest, "Empty list");
            }

            using var tran = this._domain.Db.BeginTransaction();

            var ids = updateModels.SelectReadOnlyList(m=>m.Id);

            await this._domain.Product.Update(
                updateModels
                    .Select(m => new SgProduct(m.Id, m.CategoryId, m.Name, m.ImageUrl))
                    .ToList());

            var resp = await this.UpdateProductAttributes(updateModels, ids);
            if (resp != null)
            {
                return resp;
            }

            tran.Commit();

            return ServiceResponse.Successful();
        }

        public async Task<IServiceResponse> Create(IReadOnlyList<ProductCreateModel> createModels)
        {
            if (createModels.Count < 1)
            {
                return ServiceResponse.Error(ServiceErrorCode.BadRequest, "Empty list");
            }

            using var tran = this._domain.Db.BeginTransaction();

            var ids = createModels.Select(_ => Guid.NewGuid()).ToList();

            await this._domain.Product.Create(createModels
                .Select((m,index) => new SgProduct(ids[index], m.CategoryId, m.Name, m.ImageUrl))
                .ToList());

            var resp = await this.UpdateProductAttributes(createModels, ids);
            if (resp != null)
            {
                return resp;
            }

            tran.Commit();

            return ServiceResponse.Successful();
        }

        private async Task<IServiceResponse?> UpdateProductAttributes(IReadOnlyList<ProductCreateModel> createModels, IReadOnlyList<Guid> ids)
        {
            var allAttributes = createModels.SelectMany(m => m.Values.Select(v => v.AttributeId)).Distinct().ToList();

            var attributes = await SqModelSelectBuilder
                .Select(SgAttributeTypeId.GetReader())
                .Get(t => t.AttributeId.In(allAttributes), null, i => i)
                .Query(this._domain.Db,
                    new Dictionary<Guid, SgAttributeType>(),
                    (acc, next) => acc.Append(next.Id, next.Type));

            if (attributes.Count != allAttributes.Count)
            {
                {
                    return ServiceResponse.Error(ServiceErrorCode.BadRequest, "Could not find some attributes");
                }
            }

            var p = DestructUpdateModel(createModels: createModels, ids: ids, attributes: attributes, out IServiceResponse? error);
            if (p == null)
            {
                {
                    return error ?? throw new Exception("Error should not be null here");
                }
            }

            if (p.Value.ProductAttributes.Count > 0)
            {
                await this._domain.ProductAttribute.Merge(p.Value.ProductAttributes);
            }

            if (p.Value.ProductAttributeEmpty.Count > 0)
            {
                await this._domain.ProductAttribute.Delete(p.Value.ProductAttributeEmpty);
            }

            if (p.Value.ProductAttributeSets.Count > 0)
            {
                await this._domain.ProductAttributeSet.Merge(p.Value.ProductAttributeSets);
            }

            if (p.Value.ProductAttributeSetsEmpty.Count > 0)
            {
                await this._domain.ProductAttributeSet.DeleteByProduct(p.Value.ProductAttributeSetsEmpty);
            }

            return null;
        }

        private static DestructUpdateModelResult? DestructUpdateModel(
                IReadOnlyList<ProductCreateModel> createModels, 
                IReadOnlyList<Guid> ids, 
                IReadOnlyDictionary<Guid, SgAttributeType> attributes,
                out IServiceResponse? error)
        {
            var productAttributes = new List<SgProductAttribute>();
            var productAttributeEmpty = new List<(Guid ProductId, Guid AttributeId)>();
            var productAttributeSets = new List<SgProductAttributeSet>();
            var productAttributeSetsEmpty = new List<Guid>();
            error = null;
            for (int index = 0; index < createModels.Count; index++)
            {
                var productId = ids[index];
                var m = createModels[index];
                foreach (var v in m.Values)
                {
                    if (!attributes.TryGetValue(v.AttributeId, out var attributeType))
                    {
                        throw new Exception($"Could not find attribute for {v.AttributeId}");
                    }

                    if (attributeType != SgAttributeType.SubSet)
                    {
                        if (string.IsNullOrEmpty(v.Value))
                        {
                            productAttributeEmpty.Add((productId, v.AttributeId));
                        }
                        else
                        {
                            var f = new SgProductAttributeFactory(productId, v.AttributeId, v.Value);

                            var productAttribute = attributeType.Switch<SgProductAttributeFactory, SgProductAttribute?>(ref f);
                            if (productAttribute == null)
                            {
                                error = ServiceResponse.Error(ServiceErrorCode.BadRequest, $"Incorrect format of attribute {v.AttributeId} value: {v.Value}");
                                return null;
                            }

                            productAttributes.Add(productAttribute);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(v.Value))
                        {
                            productAttributeSetsEmpty.Add(productId);
                        }
                        else
                        {
                            var rawIds = v.Value.Split(';');

                            foreach (var rawId in rawIds)
                            {
                                if (!Guid.TryParse(rawId, out var id))
                                {
                                    {
                                        error = ServiceResponse.Error(ServiceErrorCode.BadRequest,
                                            $"Incorrect format of attribute set id {rawId}");
                                        return null;
                                    }
                                }

                                productAttributeSets.Add(new SgProductAttributeSet(productId, id));
                            }
                        }
                    }
                }
            }

            return new (productAttributes, productAttributeEmpty, productAttributeSets, productAttributeSetsEmpty);
        }

        private static string Print(SgAttributeType attributeType, SgProductAttribute productAttribute)
        {
            var printer = new SgProductAttributeValuePrinter(productAttribute);
            return attributeType.Switch<SgProductAttributeValuePrinter, string>(ref printer);
        }

        private static string PrintDisplay(SgAttributeType attributeType, SgProductAttribute productAttribute, IReadOnlyDictionary<Guid, string> itemNames)
        {
            var printer = new SgProductAttributeDisplayPrinter(productAttribute, itemNames);
            return attributeType.Switch<SgProductAttributeDisplayPrinter, string>(ref printer);
        }

        private readonly struct DestructUpdateModelResult
        {
            public readonly IReadOnlyList<SgProductAttribute> ProductAttributes;
            public readonly IReadOnlyList<(Guid ProductId, Guid AttributeId)> ProductAttributeEmpty;
            public readonly IReadOnlyList<SgProductAttributeSet> ProductAttributeSets;
            public readonly IReadOnlyList<Guid> ProductAttributeSetsEmpty;

            public DestructUpdateModelResult(IReadOnlyList<SgProductAttribute> productAttributes, IReadOnlyList<(Guid ProductId, Guid AttributeId)> productAttributeEmpty, IReadOnlyList<SgProductAttributeSet> productAttributeSets, IReadOnlyList<Guid> productAttributeSetsEmpty)
            {
                this.ProductAttributes = productAttributes;
                this.ProductAttributeEmpty = productAttributeEmpty;
                this.ProductAttributeSets = productAttributeSets;
                this.ProductAttributeSetsEmpty = productAttributeSetsEmpty;
            }
        }

        readonly struct SgProductAttributeFactory : ISgAttributeTypeVisitor<SgProductAttribute?>
        {
            private readonly Guid _productId;

            private readonly Guid _attributeId;

            private readonly string _rawValue;

            public SgProductAttributeFactory(Guid productId, Guid attributeId, string rawValue)
            {
                this._productId = productId;
                this._attributeId = attributeId;
                this._rawValue = rawValue;
            }

            public SgProductAttribute? CaseBoolean()
            {
                if (!bool.TryParse(this._rawValue, out var value))
                {
                    return null;
                }

                return new SgProductAttribute(
                    productId: this._productId,
                    attributeId: this._attributeId,
                    stringValue: null,
                    intValue: null,
                    boolValue: value,
                    selectValue: null);
            }

            public SgProductAttribute? CaseInteger()
            {
                if (!int.TryParse(this._rawValue, out var value))
                {
                    return null;
                }

                return new SgProductAttribute(
                    productId: this._productId,
                    attributeId: this._attributeId,
                    stringValue: null,
                    intValue: value,
                    boolValue: null,
                    selectValue: null);
            }

            public SgProductAttribute? CaseSelect()
            {
                if (!Guid.TryParse(this._rawValue, out var value))
                {
                    return null;
                }

                return new SgProductAttribute(
                    productId: this._productId,
                    attributeId: this._attributeId,
                    stringValue: null,
                    intValue: null,
                    boolValue: null,
                    selectValue: value);
            }

            public SgProductAttribute CaseSubset()
            {
                throw new Exception("Incorrect case for subset attribute");
            }
        }

        readonly struct SgProductAttributeValuePrinter : ISgAttributeTypeVisitor<string>
        {
            private readonly SgProductAttribute _attributeValue;

            public SgProductAttributeValuePrinter(SgProductAttribute attributeValue)
            {
                this._attributeValue = attributeValue;
            }

            public string CaseBoolean()
            {
                return (this._attributeValue.BoolValue ?? throw new Exception("Bool value is expected")).ToString();
            }

            public string CaseInteger()
            {
                return (this._attributeValue.IntValue ?? throw new Exception("Guid value is expected")).ToString();
            }

            public string CaseSelect()
            {
                return (this._attributeValue.SelectValue ?? throw new Exception("Guid value is expected")).ToString();
            }

            public string CaseSubset()
            {
                throw new Exception("Incorrect case for subset attribute");
            }
        }        
        
        readonly struct SgProductAttributeDisplayPrinter : ISgAttributeTypeVisitor<string>
        {
            private readonly SgProductAttribute _attributeValue;
            private readonly IReadOnlyDictionary<Guid, string> _itemSetNames;

            public SgProductAttributeDisplayPrinter(SgProductAttribute attributeValue, IReadOnlyDictionary<Guid, string> itemSetNames)
            {
                this._attributeValue = attributeValue;
                this._itemSetNames = itemSetNames;
            }

            public string CaseBoolean()
            {
                return this._attributeValue.BoolValue ?? throw new Exception("Bool value is expected") ? "Yes" : "No";
            }

            public string CaseInteger()
            {
                return (this._attributeValue.IntValue ?? throw new Exception("Guid value is expected")).ToString();
            }

            public string CaseSelect()
            {
                var id = this._attributeValue.SelectValue ?? throw new Exception("Guid value is expected");
                if (!this._itemSetNames.TryGetValue(id, out var name))
                {
                    throw new Exception("Could not find attribute set item: " + id);
                }
                return name;
            }

            public string CaseSubset()
            {
                throw new Exception("Incorrect case for subset attribute");
            }
        }
    }
}