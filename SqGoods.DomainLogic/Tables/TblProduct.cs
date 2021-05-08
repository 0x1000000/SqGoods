using SqExpress;

namespace SqGoods.DomainLogic.Tables
{
    public class TblProduct : TableBase
    {
        public TblProduct(): this(alias: SqExpress.Alias.Auto)
        {
        }

        public TblProduct(Alias alias): base(schema: "dbo", name: "Product", alias: alias)
        {
            this.ProductId = this.CreateGuidColumn("ProductId", ColumnMeta.PrimaryKey());
            this.CategoryId = this.CreateGuidColumn("CategoryId", ColumnMeta.ForeignKey<TblCategory>(t => t.CategoryId));
            this.Name = this.CreateStringColumn(name: "Name", size: 255, isUnicode: true, isText: false, columnMeta: null);
            this.CoverUrl = this.CreateStringColumn(name: "CoverUrl", size: null, isUnicode: true, isText: false, columnMeta: null);
        }

        [SqModel("SgProduct")]
        public GuidTableColumn ProductId { get; }

        [SqModel("SgProduct")]
        public GuidTableColumn CategoryId { get; }

        [SqModel("SgProduct")]
        public StringTableColumn Name { get; }

        [SqModel("SgProduct")]
        public StringTableColumn CoverUrl { get; }
    }
}