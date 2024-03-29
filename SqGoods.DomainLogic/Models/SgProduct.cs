using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;
using SqExpress.Syntax.Select.SelectItems;

namespace SqGoods.DomainLogic.Models
{
    public record SgProduct
    {
        //Auto-generated by SqExpress Code-gen util
        public SgProduct(Guid productId, Guid categoryId, string name, string coverUrl)
        {
            this.ProductId = productId;
            this.CategoryId = categoryId;
            this.Name = name;
            this.CoverUrl = coverUrl;
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgProduct Read(ISqDataRecordReader record, TblProduct table)
        {
            return new SgProduct(productId: table.ProductId.Read(record), categoryId: table.CategoryId.Read(record), name: table.Name.Read(record), coverUrl: table.CoverUrl.Read(record));
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgProduct ReadWithPrefix(ISqDataRecordReader record, TblProduct table, string prefix)
        {
            return new SgProduct(productId: table.ProductId.Read(record, prefix + table.ProductId.ColumnName.Name), categoryId: table.CategoryId.Read(record, prefix + table.CategoryId.ColumnName.Name), name: table.Name.Read(record, prefix + table.Name.ColumnName.Name), coverUrl: table.CoverUrl.Read(record, prefix + table.CoverUrl.ColumnName.Name));
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgProduct ReadOrdinal(ISqDataRecordReader record, TblProduct table, int offset)
        {
            return new SgProduct(productId: table.ProductId.Read(record, offset), categoryId: table.CategoryId.Read(record, offset + 1), name: table.Name.Read(record, offset + 2), coverUrl: table.CoverUrl.Read(record, offset + 3));
        }

        //Auto-generated by SqExpress Code-gen util
        public Guid ProductId { get; }

        //Auto-generated by SqExpress Code-gen util
        public Guid CategoryId { get; }

        //Auto-generated by SqExpress Code-gen util
        public string Name { get; }

        //Auto-generated by SqExpress Code-gen util
        public string CoverUrl { get; }

        //Auto-generated by SqExpress Code-gen util
        public static TableColumn[] GetColumns(TblProduct table)
        {
            return new TableColumn[]{table.ProductId, table.CategoryId, table.Name, table.CoverUrl};
        }

        //Auto-generated by SqExpress Code-gen util
        public static ExprAliasedColumn[] GetColumnsWithPrefix(TblProduct table, string prefix)
        {
            return new ExprAliasedColumn[]{table.ProductId.As(prefix + table.ProductId.ColumnName.Name), table.CategoryId.As(prefix + table.CategoryId.ColumnName.Name), table.Name.As(prefix + table.Name.ColumnName.Name), table.CoverUrl.As(prefix + table.CoverUrl.ColumnName.Name)};
        }

        //Auto-generated by SqExpress Code-gen util
        public static bool IsNull(ISqDataRecordReader record, TblProduct table)
        {
            foreach (var column in GetColumns(table))
            {
                if (!record.IsDBNull(column.ColumnName.Name))
                {
                    return false;
                }
            }

            return true;
        }

        //Auto-generated by SqExpress Code-gen util
        public static bool IsNullWithPrefix(ISqDataRecordReader record, TblProduct table, string prefix)
        {
            foreach (var column in GetColumnsWithPrefix(table, prefix))
            {
                if (!record.IsDBNull(column.Alias!.Name))
                {
                    return false;
                }
            }

            return true;
        }

        //Auto-generated by SqExpress Code-gen util
        public static IRecordSetterNext GetMapping(IDataMapSetter<TblProduct, SgProduct> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId).Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.Name, s.Source.Name).Set(s.Target.CoverUrl, s.Source.CoverUrl);
        }

        //Auto-generated by SqExpress Code-gen util
        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblProduct, SgProduct> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId);
        }

        //Auto-generated by SqExpress Code-gen util
        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblProduct, SgProduct> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.Name, s.Source.Name).Set(s.Target.CoverUrl, s.Source.CoverUrl);
        }

        //Auto-generated by SqExpress Code-gen util
        public static ISqModelReader<SgProduct, TblProduct> GetReader()
        {
            return SgProductReader.Instance;
        }

        //Auto-generated by SqExpress Code-gen util
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

        //Auto-generated by SqExpress Code-gen util
        public static ISqModelUpdaterKey<SgProduct, TblProduct> GetUpdater()
        {
            return SgProductUpdater.Instance;
        }

        //Auto-generated by SqExpress Code-gen util
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