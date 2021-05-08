using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgCategory : ISgCategoryOrder, ISgCategoryIdentity
    {
        public SgCategory(Guid id, string name, int order, int? topOrder)
        {
            this.Id = id;
            this.Name = name;
            this.Order = order;
            this.TopOrder = topOrder;
        }

        public static SgCategory Read(ISqDataRecordReader record, TblCategory table)
        {
            return new SgCategory(id: table.CategoryId.Read(record), name: table.Name.Read(record), order: table.Order.Read(record), topOrder: table.TopOrder.Read(record));
        }

        public static SgCategory ReadOrdinal(ISqDataRecordReader record, TblCategory table, int offset)
        {
            return new SgCategory(id: table.CategoryId.Read(record, offset), name: table.Name.Read(record, offset + 1), order: table.Order.Read(record, offset + 2), topOrder: table.TopOrder.Read(record, offset + 3));
        }

        public Guid Id { get; }

        public string Name { get; }

        public int Order { get; }

        public int? TopOrder { get; }

        public static TableColumn[] GetColumns(TblCategory table)
        {
            return new TableColumn[]{table.CategoryId, table.Name, table.Order, table.TopOrder};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblCategory, SgCategory> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.Id).Set(s.Target.Name, s.Source.Name).Set(s.Target.Order, s.Source.Order).Set(s.Target.TopOrder, s.Source.TopOrder);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblCategory, SgCategory> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.Id);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblCategory, SgCategory> s)
        {
            return s.Set(s.Target.Name, s.Source.Name).Set(s.Target.Order, s.Source.Order).Set(s.Target.TopOrder, s.Source.TopOrder);
        }

        public static ISqModelReader<SgCategory, TblCategory> GetReader()
        {
            return SgCategoryReader.Instance;
        }

        private class SgCategoryReader : ISqModelReader<SgCategory, TblCategory>
        {
            public static SgCategoryReader Instance { get; } = new SgCategoryReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategory, TblCategory>.GetColumns(TblCategory table)
            {
                return SgCategory.GetColumns(table);
            }

            SgCategory ISqModelReader<SgCategory, TblCategory>.Read(ISqDataRecordReader record, TblCategory table)
            {
                return SgCategory.Read(record, table);
            }

            SgCategory ISqModelReader<SgCategory, TblCategory>.ReadOrdinal(ISqDataRecordReader record, TblCategory table, int offset)
            {
                return SgCategory.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgCategory, TblCategory> GetUpdater()
        {
            return SgCategoryUpdater.Instance;
        }

        private class SgCategoryUpdater : ISqModelUpdaterKey<SgCategory, TblCategory>
        {
            public static SgCategoryUpdater Instance { get; } = new SgCategoryUpdater();
            IRecordSetterNext ISqModelUpdater<SgCategory, TblCategory>.GetMapping(IDataMapSetter<TblCategory, SgCategory> dataMapSetter)
            {
                return SgCategory.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategory, TblCategory>.GetUpdateKeyMapping(IDataMapSetter<TblCategory, SgCategory> dataMapSetter)
            {
                return SgCategory.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategory, TblCategory>.GetUpdateMapping(IDataMapSetter<TblCategory, SgCategory> dataMapSetter)
            {
                return SgCategory.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}