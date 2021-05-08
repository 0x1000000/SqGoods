namespace SqGoods.Models.Filter
{
    public interface IFilterBoolModelVisitor<out TRes>
    {
        TRes CaseAnd(FilterAndModel model);
        TRes CaseOr(FilterOrModel model);
        TRes CaseSelectPredicate(FilterSelectPredicateModel model);
        TRes CaseSetPredicate(FilterSetPredicateModel model);
        TRes CaseBoolPredicate(FilterBoolPredicateModel model);
        TRes CaseIntBetweenPredicate(FilterIntBetweenPredicateModel model);
    }
}