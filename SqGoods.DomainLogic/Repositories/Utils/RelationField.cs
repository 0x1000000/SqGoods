using SqExpress;
using SqExpress.Syntax.Boolean;
using SqExpress.Syntax.Names;
using SqExpress.Syntax.Value;

namespace SqGoods.DomainLogic.Repositories.Utils
{
    public class RelationField
    {
        private readonly ExprColumn _column;

        public RelationField(ExprColumn column)
        {
            this._column = column;
        }

        public ExprBoolean In(ExprValue item, params ExprValue[] items)
        {
            return this._column.In(item, items);
        }
    }
}