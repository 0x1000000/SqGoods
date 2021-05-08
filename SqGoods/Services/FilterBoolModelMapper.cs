using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SqExpress;
using SqExpress.Syntax.Boolean;
using SqExpress.Syntax.Names;
using SqGoods.Models.Filter;

namespace SqGoods.Services
{
    public class FilterBoolModelMapper : IFilterBoolModelVisitor<(ExprBoolean? Expr, string? Error)>
    {
        public static readonly FilterBoolModelMapper Instance = new();

        private FilterBoolModelMapper() { }

        public (ExprBoolean? Expr, string? Error) CaseAnd(FilterAndModel model)
        {
            if (model.Items.Count == 0)
            {
                return (null, "'AND' items cannot be empty");
            }
            if (model.Items.Count == 1)
            {
                return model.Items[0].Accept(this);
            }

            var buffer = new List<ExprBoolean>(model.Items.Count);
            foreach (var item in model.Items)
            {
                var res = item.Accept(this);
                if (res.Error != null)
                {
                    return (null, res.Error);
                }

                buffer.Add(res.Expr ?? throw new NullReferenceException("Expression cannot be null here"));
            }
            return (buffer.JoinAsAnd(), null);
        }

        public (ExprBoolean? Expr, string? Error) CaseOr(FilterOrModel model)
        {
            if (model.Items.Count == 0)
            {
                return (null, "'OR' items cannot be empty");
            }
            if (model.Items.Count == 1)
            {
                return model.Items[0].Accept(this);
            }

            var buffer = new List<ExprBoolean>(model.Items.Count);
            foreach (var item in model.Items)
            {
                var res = item.Accept(this);
                if (res.Error != null)
                {
                    return (null, res.Error);
                }

                buffer.Add(res.Expr ?? throw new NullReferenceException("Expression cannot be null here"));
            }
            return (buffer.JoinAsOr(), null);
        }

        public (ExprBoolean? Expr, string? Error) CaseSelectPredicate(FilterSelectPredicateModel model)
        {
            if (!CheckColumn(model, out var column, out var error))
            {
                return (null, error);
            }
            return (column == model.SelectValueId, null);
        }

        public (ExprBoolean? Expr, string? Error) CaseSetPredicate(FilterSetPredicateModel model)
        {
            if (model.SelectValueIds.Count < 1)
            {
                return (null, "Set predicate should have at least one value id");
            }
            if (!CheckColumn(model, out var column, out var error))
            {
                return (null, error);
            }
            return (column.In(model.SelectValueIds), null);
        }

        public (ExprBoolean? Expr, string? Error) CaseBoolPredicate(FilterBoolPredicateModel model)
        {
            if (!CheckColumn(model, out var column, out var error))
            {
                return (null, error);
            }
            return (column == model.Value, null);
        }

        public (ExprBoolean? Expr, string? Error) CaseIntBetweenPredicate(FilterIntBetweenPredicateModel model)
        {
            var from = model.From;
            var to = model.To;
            if (from == null && to == null)
            {
                return (null, "Incorrect range");
            }

            if (!CheckColumn(model, out var column, out var error))
            {
                return (null, error);
            }
            if (from.HasValue && to.HasValue)
            {
                return (column >= from.Value & column <= to.Value, null);
            }
            if (from.HasValue)
            {
                return (column >= from.Value, null);
            }
            if (to.HasValue)
            {
                return (column <= to.Value, null);
            }

            throw new Exception("Incorrect case");
        }

        public bool CheckColumn(FilterPredicateModel predicate,[NotNullWhen(true)] out ExprColumn? column, [NotNullWhen(false)] out string? error)
        {
            column = null;
            error = null;
            if (predicate.AttributeId == Guid.Empty)
            {
                error = "AttributeId cannot be empty";
                return false;
            }

            column = SqQueryBuilder.Column(predicate.AttributeId.ToString());
            return true;
        }
    }
}