using SqExpress;
using SqGoods.DomainLogic.Models;

namespace SqGoods.DomainLogic.Tables
{
    public class TblAttributeSet : TableBase
    {
        public TblAttributeSet(): this(alias: SqExpress.Alias.Auto)
        {
        }

        public TblAttributeSet(Alias alias): base(schema: "dbo", name: "AttributeSet", alias: alias)
        {
            this.AttributeSetId = this.CreateGuidColumn("AttributeSetId", ColumnMeta.PrimaryKey());
            this.AttributeId = this.CreateGuidColumn("AttributeId", ColumnMeta.ForeignKey<TblAttribute>(t => t.AttributeId));
            this.Name = this.CreateStringColumn(name: "Name", size: 255, isUnicode: true, isText: false, columnMeta: null);
            this.Order = this.CreateInt32Column("Order");
            this.AddUniqueIndex(this.AttributeId, this.AttributeSetId);
        }

        [SqModel("SgAttributeSetId")]
        [SqModel("SgAttributeItem")]
        [SqModel("SgAttributeItemName")]
        public GuidTableColumn AttributeSetId { get; }

        [SqModel("SgAttributeSetId")]
        [SqModel("SgAttributeItem")]
        public GuidTableColumn AttributeId { get; }

        [SqModel("SgAttributeItem")]
        [SqModel("SgAttributeItemName")]
        public StringTableColumn Name { get; }

        [SqModel("SgAttributeItem")]
        public Int32TableColumn Order { get; }
    }
}