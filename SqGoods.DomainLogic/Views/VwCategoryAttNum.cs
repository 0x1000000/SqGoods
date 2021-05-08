using SqExpress;
using SqGoods.DomainLogic.Tables;
using static SqExpress.SqQueryBuilder;

namespace SqGoods.DomainLogic.Views
{
    public class VwCategoryAttNum : DerivedTableBase
    {
        [SqModel("SgCategoryAttNum")]
        public GuidCustomColumn CategoryId { get; set; }

        [SqModel("SgCategoryAttNum")]
        public Int32CustomColumn AttributesCount { get; set; }

        public VwCategoryAttNum()
        {
            this.CategoryId = this.CreateGuidColumn("CategoryId");
            this.AttributesCount = this.CreateInt32Column("AttributesCount");
        }

        protected override IExprSubQuery CreateQuery()
        {
            var tblCategory = AllTables.GetCategory();
            var tblCategoryAttribute = AllTables.GetCategoryAttribute();

            return Select(
                    tblCategory.CategoryId.As(this.CategoryId), 
                    CountOne().As(this.AttributesCount))
                .From(tblCategory)
                .InnerJoin(tblCategoryAttribute, on: tblCategoryAttribute.CategoryId == tblCategory.CategoryId)
                .GroupBy(tblCategory.CategoryId)
                .Done();
        }
    }
}