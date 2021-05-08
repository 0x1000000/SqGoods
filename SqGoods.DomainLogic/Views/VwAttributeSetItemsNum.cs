using SqExpress;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Tables;

namespace SqGoods.DomainLogic.Views
{
    public class VwAttributeSetItemsNum : DerivedTableBase
    {
        [SqModel("SgAttributeSetItemsNum")]
        public GuidCustomColumn AttributeId { get; set; }

        [SqModel("SgAttributeSetItemsNum")]
        public Int32CustomColumn SetItemsNum { get; set; }

        public VwAttributeSetItemsNum()
        {
            this.AttributeId = this.CreateGuidColumn(nameof(this.AttributeId));
            this.SetItemsNum = this.CreateInt32Column(nameof(this.SetItemsNum));
        }

        protected override IExprSubQuery CreateQuery()
        {
            var tblAttribute = AllTables.GetAttribute();
            var tblAttributeItems = AllTables.GetAttributeSet();

            return SqQueryBuilder.Select(
                    tblAttribute.AttributeId.As(this.AttributeId), 
                    SqQueryBuilder.CountOne().As(this.SetItemsNum))
                .From(tblAttribute)
                .LeftJoin(tblAttributeItems, @on: tblAttributeItems.AttributeId == tblAttribute.AttributeId)
                .Where(tblAttribute.Type == (int)SgAttributeType.Select | tblAttribute.Type == (int)SgAttributeType.SubSet)
                .GroupBy(tblAttribute.AttributeId)
                .Done();
        }
    }
}