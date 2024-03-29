using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Views;
using SqExpress.Syntax.Names;
using System.Collections.Generic;
using SqExpress.Syntax.Select.SelectItems;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttributeCatsNum
    {
        //Auto-generated by SqExpress Code-gen util
        public SgAttributeCatsNum(Guid attributeId, int categoriesCount)
        {
            this.AttributeId = attributeId;
            this.CategoriesCount = categoriesCount;
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgAttributeCatsNum Read(ISqDataRecordReader record, VwAttributeCatsNum table)
        {
            return new SgAttributeCatsNum(attributeId: table.AttributeId.Read(record), categoriesCount: table.CategoriesCount.Read(record));
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgAttributeCatsNum ReadWithPrefix(ISqDataRecordReader record, VwAttributeCatsNum table, string prefix)
        {
            return new SgAttributeCatsNum(attributeId: table.AttributeId.Read(record, prefix + table.AttributeId.ColumnName.Name), categoriesCount: table.CategoriesCount.Read(record, prefix + table.CategoriesCount.ColumnName.Name));
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgAttributeCatsNum ReadOrdinal(ISqDataRecordReader record, VwAttributeCatsNum table, int offset)
        {
            return new SgAttributeCatsNum(attributeId: table.AttributeId.Read(record, offset), categoriesCount: table.CategoriesCount.Read(record, offset + 1));
        }

        //Auto-generated by SqExpress Code-gen util
        public Guid AttributeId { get; }

        //Auto-generated by SqExpress Code-gen util
        public int CategoriesCount { get; }

        //Auto-generated by SqExpress Code-gen util
        public static ExprColumn[] GetColumns(VwAttributeCatsNum table)
        {
            return new ExprColumn[]{table.AttributeId, table.CategoriesCount};
        }

        //Auto-generated by SqExpress Code-gen util
        public static ExprAliasedColumn[] GetColumnsWithPrefix(VwAttributeCatsNum table, string prefix)
        {
            return new ExprAliasedColumn[]{table.AttributeId.As(prefix + table.AttributeId.ColumnName.Name), table.CategoriesCount.As(prefix + table.CategoriesCount.ColumnName.Name)};
        }

        //Auto-generated by SqExpress Code-gen util
        public static bool IsNull(ISqDataRecordReader record, VwAttributeCatsNum table)
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
        public static bool IsNullWithPrefix(ISqDataRecordReader record, VwAttributeCatsNum table, string prefix)
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
        public static ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum> GetReader()
        {
            return SgAttributeCatsNumReader.Instance;
        }

        //Auto-generated by SqExpress Code-gen util
        private class SgAttributeCatsNumReader : ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum>
        {
            public static SgAttributeCatsNumReader Instance { get; } = new SgAttributeCatsNumReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum>.GetColumns(VwAttributeCatsNum table)
            {
                return SgAttributeCatsNum.GetColumns(table);
            }

            SgAttributeCatsNum ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum>.Read(ISqDataRecordReader record, VwAttributeCatsNum table)
            {
                return SgAttributeCatsNum.Read(record, table);
            }

            SgAttributeCatsNum ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum>.ReadOrdinal(ISqDataRecordReader record, VwAttributeCatsNum table, int offset)
            {
                return SgAttributeCatsNum.ReadOrdinal(record, table, offset);
            }
        }
    }
}