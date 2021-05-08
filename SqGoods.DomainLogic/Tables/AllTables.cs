using SqExpress;

namespace SqGoods.DomainLogic.Tables
{
    internal static class AllTables
    {
        public static TableBase[] BuildAllTableList() => new TableBase[]
        {
            GetAttribute(Alias.Empty), 
            GetCategory(Alias.Empty), 
            GetAttributeSet(Alias.Empty),
            GetCategoryAttribute(Alias.Empty), 
            GetProduct(Alias.Empty),
            GetProductAttribute(Alias.Empty),
            GetProductAttributeSet(Alias.Empty)
        };
        public static TblAttribute GetAttribute(Alias alias) => new TblAttribute(alias);
        public static TblAttribute GetAttribute() => new TblAttribute(Alias.Auto);
        public static TblCategory GetCategory(Alias alias) => new TblCategory(alias);
        public static TblCategory GetCategory() => new TblCategory(Alias.Auto);
        public static TblAttributeSet GetAttributeSet(Alias alias) => new TblAttributeSet(alias);
        public static TblAttributeSet GetAttributeSet() => new TblAttributeSet(Alias.Auto);
        public static TblCategoryAttribute GetCategoryAttribute(Alias alias) => new TblCategoryAttribute(alias);
        public static TblCategoryAttribute GetCategoryAttribute() => new TblCategoryAttribute(Alias.Auto);
        public static TblProduct GetProduct(Alias alias) => new TblProduct(alias);
        public static TblProduct GetProduct() => new TblProduct(Alias.Auto);
        public static TblProductAttribute GetProductAttribute(Alias alias) => new TblProductAttribute(alias);
        public static TblProductAttribute GetProductAttribute() => new TblProductAttribute(Alias.Auto);
        public static TblProductAttributeSet GetProductAttributeSet(Alias alias) => new TblProductAttributeSet(alias);
        public static TblProductAttributeSet GetProductAttributeSet() => new TblProductAttributeSet(Alias.Auto);
    }
}