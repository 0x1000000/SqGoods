using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgProduct
    {
        public SgProduct(Guid productId, Guid categoryId, string name, string coverUrl)
        {
            this.ProductId = productId;
            this.CategoryId = categoryId;
            this.Name = name;
            this.CoverUrl = coverUrl;
        }

        public static SgProduct Read(ISqDataRecordReader record, TblProduct table)
        {
            return new SgProduct(productId: table.ProductId.Read(record), categoryId: table.CategoryId.Read(record), name: table.Name.Read(record), coverUrl: table.CoverUrl.Read(record));
        }

        public static SgProduct ReadOrdinal(ISqDataRecordReader record, TblProduct table, int offset)
        {
            return new SgProduct(productId: table.ProductId.Read(record, offset), categoryId: table.CategoryId.Read(record, offset + 1), name: table.Name.Read(record, offset + 2), coverUrl: table.CoverUrl.Read(record, offset + 3));
        }

        public Guid ProductId { get; }

        public Guid CategoryId { get; }

        public string Name { get; }

        public string CoverUrl { get; }

        public static TableColumn[] GetColumns(TblProduct table)
        {
            return new TableColumn[]{table.ProductId, table.CategoryId, table.Name, table.CoverUrl};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblProduct, SgProduct> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId).Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.Name, s.Source.Name).Set(s.Target.CoverUrl, s.Source.CoverUrl);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblProduct, SgProduct> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblProduct, SgProduct> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.Name, s.Source.Name).Set(s.Target.CoverUrl, s.Source.CoverUrl);
        }

        public static ISqModelReader<SgProduct, TblProduct> GetReader()
        {
            return SgProductReader.Instance;
        }

        private class SgProductReader : ISqModelReader<SgProduct, TblProduct>
        {
            public static SgProductReader Instance { get; } = new SgProductReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgProduct, TblProduct>.GetColumns(TblProduct table)
            {
                return SgProduct.GetColumns(table);
            }

            SgProduct ISqModelReader<SgProduct, TblProduct>.Read(ISqDataRecordReader record, TblProduct table)
            {
                return SgProduct.Read(record, table);
            }

            SgProduct ISqModelReader<SgProduct, TblProduct>.ReadOrdinal(ISqDataRecordReader record, TblProduct table, int offset)
            {
                return SgProduct.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgProduct, TblProduct> GetUpdater()
        {
            return SgProductUpdater.Instance;
        }

        private class SgProductUpdater : ISqModelUpdaterKey<SgProduct, TblProduct>
        {
            public static SgProductUpdater Instance { get; } = new SgProductUpdater();
            IRecordSetterNext ISqModelUpdater<SgProduct, TblProduct>.GetMapping(IDataMapSetter<TblProduct, SgProduct> dataMapSetter)
            {
                return SgProduct.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgProduct, TblProduct>.GetUpdateKeyMapping(IDataMapSetter<TblProduct, SgProduct> dataMapSetter)
            {
                return SgProduct.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgProduct, TblProduct>.GetUpdateMapping(IDataMapSetter<TblProduct, SgProduct> dataMapSetter)
            {
                return SgProduct.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}