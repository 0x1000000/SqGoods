using System;
using System.Collections.Generic;
using SqExpress;
using SqExpress.Syntax.Boolean;
using SqExpress.Syntax.Boolean.Predicate;
using SqExpress.Syntax.Names;
using SqExpress.Syntax.Value;

namespace SqGoods.DomainLogic.Repositories.Utils
{
    internal abstract class RelationFieldsBase<TTable>
    {
        private readonly ExprTableAlias _alias = new ExprTableAlias(new ExprAlias("REL"));

        private readonly IDictionary<ExprColumn, Func<TTable, IReadOnlyList<ExprValue>, ExprBoolean>> _columns = new Dictionary<ExprColumn, Func<TTable, IReadOnlyList<ExprValue>, ExprBoolean>>();

        protected RelationField Create(string name, Func<TTable, IReadOnlyList<ExprValue>, ExprBoolean> factory)
        {
            ExprColumn column = new ExprColumn(this._alias, name);
            if (this._columns.ContainsKey(column))
            {
                throw new Exception("Duplicate column name");
            }
            this._columns.Add(column, factory);
            return new RelationField(column);
        }

        public ExprBoolean ReplaceFields(TTable table, ExprBoolean filter)
        {
            return (ExprBoolean)filter.SyntaxTree()
                .Modify<ExprInValues>(e =>
                    e.TestExpression is ExprColumn column && this._columns.TryGetValue(column, out var factory)
                        ? factory(table, e.Items)
                        : e)!;
        }
    }
}