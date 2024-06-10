using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqExpress.Syntax.Boolean;
using SqExpress.Syntax.Boolean.Predicate;
using SqExpress.Syntax.Names;
using SqExpress.Syntax.Select;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Tables;

namespace SqGoods.DomainLogic.Repositories.Utils
{
    public interface IProductValuesView : IExprTableSource
    {
        ExprColumn ProductId { get; }
    }

    internal class FilterProcessor
    {
        public static async Task<(ExprBoolean Filter, IProductValuesView? ProductValuesView)> Prepare(ISqDatabase database, ExprColumn hostProduct,ExprBoolean filter)
        {
            var columns = filter.SyntaxTree()
                .Descendants()
                .OfType<ExprColumn>()
                .Distinct()
                .Select(c => Guid.TryParse(c.ColumnName.Name, out var guid) ? (Guid?)guid : null)
                .Where(g => g.HasValue)
                .Select(g => g!.Value)
                .ToList();

            var types = await SqModelSelectBuilder.Select(SgAttributeTypeId.GetReader())
                .Get(t => t.AttributeId.In(columns), null, i => i)
                .QueryDict(database, i => (i.Id, i.Type));

            if (types.Count < 1)
            {
                return (filter, null);
            }

            if (types.Count != columns.Count)
            {
                throw new Exception("Could not find the following attributes: " +
                                    string.Join(',',
                                        columns.Select(c => !types.ContainsKey(c)).Select(c => c.ToString())));
            }

            var singleValueCols = new List<(Guid Id, SgAttributeType Type)>();
            var setCols = new HashSet<Guid>();
            foreach (var column in columns)
            {
                var colType = types[column];
                if (colType != SgAttributeType.SubSet)
                {
                    singleValueCols.Add((column, colType));
                }
                else
                {
                    setCols.Add(column);
                }
            }

            IProductValuesView? view = null;
            if (singleValueCols.Count > 0)
            {
                view = new ProductValuesView(singleValueCols);
            }

            if (setCols.Count > 0)
            {
                filter = (ExprBoolean)filter.SyntaxTree()
                    .Modify<ExprInValues>(exprIn =>
                    {
                        if (exprIn.TestExpression is ExprColumn column
                            && Guid.TryParse(column.ColumnName.Name, out var guidCol)
                            && setCols.Contains(guidCol))
                        {
                            var tblAttributeSet = AllTables.GetAttributeSet();
                            var tblProductAttributeSet = AllTables.GetProductAttributeSet();

                            return SqQueryBuilder.Exists(SqQueryBuilder
                                .SelectOne()
                                .From(tblProductAttributeSet)
                                .InnerJoin(tblAttributeSet,
                                    on: tblAttributeSet.AttributeSetId == tblProductAttributeSet.AttributeSetId)
                                .Where(
                                    tblProductAttributeSet.ProductId == hostProduct &
                                    tblAttributeSet.AttributeId == guidCol &
                                    tblProductAttributeSet.AttributeSetId.In(exprIn.Items)));
                        }
                        return exprIn;
                    })!;
            }

            return (filter, view);
        }

        private class ProductValuesView : DerivedTableBase, IProductValuesView
        {
            private readonly IExprSubQuery _subQuery;

            public ExprColumn ProductId { get; }

            public ProductValuesView(IReadOnlyList<(Guid Id, SgAttributeType Type)> columns)
            {
                var tProduct = AllTables.GetProduct();

                this.ProductId = tProduct.ProductId.WithSource(this.Alias);

                var selectList = new List<IExprSelecting>(columns.Count + 1)
                {
                    tProduct.ProductId
                };

                var tblProductAttribute = AllTables.GetProductAttribute();

                var joiner = SqQueryBuilder.Select(selectList).From(tProduct);

                foreach (var column in columns)
                {
                    ExprColumn attributeColumn = column.Type switch
                    {
                        SgAttributeType.Boolean => tblProductAttribute.BoolValue,
                        SgAttributeType.Integer => tblProductAttribute.IntValue,
                        SgAttributeType.Select => tblProductAttribute.GuidValue,
                        SgAttributeType.SubSet => throw new Exception("Set attributes require cannot be used here"),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    var nextJoinedProductAttribute = AllTables.GetProductAttribute();

                    var selecting = attributeColumn
                        .WithSource(nextJoinedProductAttribute.Alias)
                        .As(column.Id.ToString("D"));

                    selectList.Add(selecting);

                    joiner = joiner.LeftJoin(
                        nextJoinedProductAttribute, 
                        on: 
                            nextJoinedProductAttribute.ProductId == tProduct.ProductId
                            &
                            nextJoinedProductAttribute.AttributeId == column.Id);
                }
                this._subQuery = joiner.Done();
            }

            protected override IExprSubQuery CreateQuery() => this._subQuery;
        }
    }
}