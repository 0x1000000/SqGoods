using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqExpress;
using SqGoods.DomainLogic;
using SqGoods.DomainLogic.Models;
using SqGoods.Infrastructure;
using SqGoods.Models;
using SqGoods.Models.Fields;
using SqGoods.Models.Filter;

namespace SqGoods.Services
{
    public class CatalogService
    {
        private readonly IDomainLogic _domain;

        public CatalogService(IDomainLogic domain)
        {
            this._domain = domain;
        }

        public async Task<IReadOnlyList<IdNameModel>> GetTopCategories()
        {
            var page = await SqModelSelectBuilder.Select(SgCategoryName.GetReader())
                .Find(0,
                    10,
                    t => SqQueryBuilder.IsNotNull(t.TopOrder),
                    t => t.TopOrder,
                    c => new IdNameModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                .QueryPage(this._domain.Db);

            return page.Items;
        }

        public Task<IServiceResponse<DataPageModel<ProductListModel>>> FindProducts(Guid categoryId, int offset, int pageSize, FilterBoolModel? filter)
        {
            var productService = new ProductService(this._domain);
            return productService.Find(offset, pageSize, categoryId, filter);
        }

        public async Task<IReadOnlyList<FieldDescriptorModel>> GetCategoryAttributes(Guid categoryId)
        {
            var service = new CategoriesService(this._domain);
            var models = await service.GetFormFields(categoryId);
            return models.SelectReadOnlyList(i => i.Descriptor!);
        }
    }
}