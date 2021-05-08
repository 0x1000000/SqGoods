using SqExpress;
using SqGoods.DomainLogic.Models;

namespace SqGoods.DomainLogic.Tables
{
    public class TblAttribute : TableBase
    {
        public TblAttribute(): this(alias: SqExpress.Alias.Auto)
        {
        }

        public TblAttribute(Alias alias): base(schema: "dbo", name: "Attribute", alias: alias)
        {
            this.AttributeId = this.CreateGuidColumn("AttributeId", ColumnMeta.PrimaryKey());
            this.Name = this.CreateStringColumn(name: "Name", size: 255, isUnicode: true, isText: false, columnMeta: null);
            this.Type = this.CreateInt16Column("Type", null);
            this.Unit = this.CreateNullableStringColumn("Unit", size: 255, isUnicode: true);
        }

        [SqModel("SgAttribute", PropertyName = "Id")]
        [SqModel("SgAttributeId", PropertyName = "Id")]
        [SqModel("SgAttributeTypeId", PropertyName = "Id")]
        public GuidTableColumn AttributeId { get; }

        [SqModel("SgAttribute")]
        public StringTableColumn Name { get; }

        [SqModel("SgAttribute", PropertyName = "Type", CastType = typeof(SgAttributeType))]
        [SqModel("SgAttributeTypeId", PropertyName = "Type", CastType = typeof(SgAttributeType))]
        public Int16TableColumn Type { get; }

        [SqModel("SgAttribute")]
        public NullableStringTableColumn Unit { get; }
    }
}