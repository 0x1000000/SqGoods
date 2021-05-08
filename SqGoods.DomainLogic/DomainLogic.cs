using SqExpress.DataAccess;
using SqGoods.DomainLogic.Repositories;

namespace SqGoods.DomainLogic
{
    public interface IDomainLogic
    {
        ISqDatabase Db { get; }
        ISgCategoryRepository Category { get; }
        ISgAttributeRepository Attribute { get; }
        ISgCategoryAttributeRepository CategoryAttribute { get; }
        ISgAttributeSetRepository AttributeSet { get; }
        ISgProductRepository Product { get; }
        ISgProductAttributeRepository ProductAttribute { get; }
        ISgProductAttributeSetRepository ProductAttributeSet { get; }
    }

    internal class DomainLogic : IDomainLogic
    {
        private ISgCategoryRepository? _categoryRepository;
        private ISgAttributeRepository? _attributeRepository;
        private ISgCategoryAttributeRepository? _categoryAttributeRepository;
        private ISgAttributeSetRepository? _attributeSetRepository;
        private ISgProductRepository? _productRepository;
        private ISgProductAttributeRepository? _productAttributeRepository;
        private ISgProductAttributeSetRepository? _productAttributeSetRepository;

        public ISqDatabase Db { get; }

        public DomainLogic(ISqDatabase db)
        {
            this.Db = db;
        }

        public ISgCategoryRepository Category => 
            this._categoryRepository ??= new SgCategoryRepository(this.Db);

        public ISgAttributeRepository Attribute => 
            this._attributeRepository ??= new SgAttributeRepository(this.Db);

        public ISgCategoryAttributeRepository CategoryAttribute =>
            this._categoryAttributeRepository ??= new SgCategoryAttributeRepository(this.Db);

        public ISgAttributeSetRepository AttributeSet =>
            this._attributeSetRepository ??= new SgAttributeSetRepository(this.Db);

        public ISgProductRepository Product =>
            this._productRepository ??= new SgProductRepository(this.Db);

        public ISgProductAttributeRepository ProductAttribute =>
            this._productAttributeRepository ??= new SgProductAttributeRepository(this.Db);

        public ISgProductAttributeSetRepository ProductAttributeSet =>
            this._productAttributeSetRepository ??= new SgProductAttributeSetRepository(this.Db);


    }
}