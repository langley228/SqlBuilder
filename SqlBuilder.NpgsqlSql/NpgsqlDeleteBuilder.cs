using System;
using System.Linq.Expressions;

namespace SqlBuilder.NpgsqlSql
{
    internal class NpgsqlDeleteBuilder<TModel> :
        AbstractNpgsqlBuilder<TModel>,
        ISqlRawDeleteBuilder<TModel> where TModel : class
    {
        public NpgsqlDeleteBuilder(AbstractNpgsqlBuilder<TModel> builder) : base(builder)
        {
        }

        public ISqlRawExecuteBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate)
        {
            _sb.AppendLine($" WHERE ")
               .AppendLine(GetExpressionWhere(predicate.Body));
            return new NpgsqlExecuteBuilder<TModel>(this);
        }
    }
}