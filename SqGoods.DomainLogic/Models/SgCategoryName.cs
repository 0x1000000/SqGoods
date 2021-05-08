using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgCategoryName
    {
        public SgCategoryName(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public static SgCategoryName Read(ISqDataRecordReader record, TblCategory table)
        {
            return new SgCategoryName(id: table.CategoryId.Read(record), name: table.Name.Read(record));
        }

        public static SgCategoryName ReadOrdinal(ISqDataRecordReader record, TblCategory table, int offset)
        {
            return new SgCategoryName(id: table.CategoryId.Read(record, offset), name: table.Name.Read(record, offset + 1));
        }

        public Guid Id { get; }

        public string Name { get; }

        public static TableColumn[] GetColumns(TblCategory table)
        {
            return new TableColumn[]{table.CategoryId, table.Name};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblCategory, SgCategoryName> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.Id).Set(s.Target.Name, s.Source.Name);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblCategory, SgCategoryName> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.Id);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblCategory, SgCategoryName> s)
        {
            return s.Set(s.Target.Name, s.Source.Name);
        }

        public static ISqModelReader<SgCategoryName, TblCategory> GetReader()
        {
            return SgCategoryNameReader.Instance;
        }

        private class SgCategoryNameReader : ISqModelReader<SgCategoryName, TblCategory>
        {
            public static SgCategoryNameReader Instance { get; } = new SgCategoryNameReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategoryName, TblCategory>.GetColumns(TblCategory table)
            {
                return SgCategoryName.GetColumns(table);
            }

            SgCategoryName ISqModelReader<SgCategoryName, TblCategory>.Read(ISqDataRecordReader record, TblCategory table)
            {
                return SgCategoryName.Read(record, table);
            }

            SgCategoryName ISqModelReader<SgCategoryName, TblCategory>.ReadOrdinal(ISqDataRecordReader record, TblCategory table, int offset)
            {
                return SgCategoryName.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgCategoryName, TblCategory> GetUpdater()
        {
            return SgCategoryNameUpdater.Instance;
        }

        private class SgCategoryNameUpdater : ISqModelUpdaterKey<SgCategoryName, TblCategory>
        {
            public static SgCategoryNameUpdater Instance { get; } = new SgCategoryNameUpdater();
            IRecordSetterNext ISqModelUpdater<SgCategoryName, TblCategory>.GetMapping(IDataMapSetter<TblCategory, SgCategoryName> dataMapSetter)
            {
                return SgCategoryName.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryName, TblCategory>.GetUpdateKeyMapping(IDataMapSetter<TblCategory, SgCategoryName> dataMapSetter)
            {
                return SgCategoryName.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryName, TblCategory>.GetUpdateMapping(IDataMapSetter<TblCategory, SgCategoryName> dataMapSetter)
            {
                return SgCategoryName.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}