using SqExpress;

namespace SqGoods.DomainLogic.Tables
{
    public class TblProductAttributeSet : TableBase
    {
        public TblProductAttributeSet() : this(alias: SqExpress.Alias.Auto)
        {
        }

        public TblProductAttributeSet(Alias alias) : base(schema: "dbo", name: "ProductAttributeSet", alias: alias)
        {
            this.ProductId = this.CreateGuidColumn("ProductId", ColumnMeta.PrimaryKey().ForeignKey<TblProduct>(t=>t.ProductId));
            this.AttributeSetId = this.CreateGuidColumn("AttributeSetId", ColumnMeta.PrimaryKey().ForeignKey<TblAttributeSet>(t => t.AttributeSetId));
        }

        [SqModel("SgProductAttributeSet")]
        public GuidTableColumn ProductId { get; }

        [SqModel("SgProductAttributeSet")]
        public GuidTableColumn AttributeSetId { get; }
    }
}