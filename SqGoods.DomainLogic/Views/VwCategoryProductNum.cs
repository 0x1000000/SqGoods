using SqExpress;
using SqGoods.DomainLogic.Tables;

namespace SqGoods.DomainLogic.Views
{
    public class VwCategoryProductNum : DerivedTableBase
    {
        [SqModel("SgCategoryProductNum")]
        public GuidCustomColumn CategoryId { get; set; }

        [SqModel("SgCategoryProductNum")]
        public Int32CustomColumn ProductCount { get; set; }

        public VwCategoryProductNum()
        {
            this.CategoryId = this.CreateGuidColumn("CategoryId");
            this.ProductCount = this.CreateInt32Column("ProductCount");
        }

        protected override IExprSubQuery CreateQuery()
        {
            var tblCategory = AllTables.GetCategory();
            var tblProduct = AllTables.GetProduct();

            return SqQueryBuilder.Select(
                    tblCategory.CategoryId.As(this.CategoryId), 
                    SqQueryBuilder.CountOne().As(this.ProductCount))
                .From(tblCategory)
                .InnerJoin(tblProduct, @on: tblProduct.CategoryId == tblCategory.CategoryId)
                .GroupBy(tblCategory.CategoryId)
                .Done();
        }
    }
}