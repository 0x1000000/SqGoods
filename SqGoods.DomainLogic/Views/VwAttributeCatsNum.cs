using SqExpress;
using SqGoods.DomainLogic.Tables;

namespace SqGoods.DomainLogic.Views
{
    public class VwAttributeCatsNum : DerivedTableBase
    {
        [SqModel("SgAttributeCatsNum")]
        public GuidCustomColumn AttributeId { get; set; }

        [SqModel("SgAttributeCatsNum")]
        public Int32CustomColumn CategoriesCount { get; set; }

        public VwAttributeCatsNum()
        {
            this.AttributeId = this.CreateGuidColumn(nameof(this.AttributeId));
            this.CategoriesCount = this.CreateInt32Column(nameof(this.CategoriesCount));
        }

        protected override IExprSubQuery CreateQuery()
        {
            var tblAttribute = AllTables.GetAttribute();
            var tblCategoryAttribute = AllTables.GetCategoryAttribute();

            return SqQueryBuilder.Select(
                    tblAttribute.AttributeId.As(this.AttributeId), 
                    SqQueryBuilder.CountOne().As(this.CategoriesCount))
                .From(tblAttribute)
                .LeftJoin(tblCategoryAttribute, @on: tblCategoryAttribute.AttributeId == tblAttribute.AttributeId)
                .GroupBy(tblAttribute.AttributeId)
                .Done();
        }
    }
}