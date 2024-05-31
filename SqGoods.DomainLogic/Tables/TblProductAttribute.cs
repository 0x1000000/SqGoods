using SqExpress;

namespace SqGoods.DomainLogic.Tables
{
    public class TblProductAttribute : TableBase
    {
        public TblProductAttribute() : this(alias: SqExpress.Alias.Auto)
        {
        }

        public TblProductAttribute(Alias alias) : base(schema: "dbo", name: "ProductAttribute", alias: alias)
        {
            this.ProductId = this.CreateGuidColumn("ProductId", ColumnMeta.PrimaryKey().ForeignKey<TblProduct>(t=>t.ProductId));
            this.AttributeId = this.CreateGuidColumn("AttributeId", ColumnMeta.PrimaryKey().ForeignKey<TblAttribute>(t => t.AttributeId));
            this.StringValue  = this.CreateNullableStringColumn("StringValue", 2048, isUnicode: true);
            this.IntValue = this.CreateNullableInt32Column("IntValue");
            this.BoolValue = this.CreateNullableBooleanColumn("BoolValue");
            this.SelectValue = this.CreateNullableGuidColumn("GuidValue");
        }

        [SqModel("SgProductAttribute")]
        public GuidTableColumn ProductId { get; }

        [SqModel("SgProductAttribute")]
        public GuidTableColumn AttributeId { get; }

        [SqModel("SgProductAttribute")]
        public NullableStringTableColumn StringValue { get; }

        [SqModel("SgProductAttribute")]
        public NullableInt32TableColumn IntValue { get; }

        [SqModel("SgProductAttribute")]
        public NullableBooleanTableColumn BoolValue { get; }

        [SqModel("SgProductAttribute")]
        public NullableGuidTableColumn SelectValue { get; }
    }
}