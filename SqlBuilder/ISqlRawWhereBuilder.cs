using System;
using System.Linq.Expressions;

namespace SqlBuilder
{
    public interface ISqlRawWhereBuilder<TModel> where TModel : class
    {
        /// <summary>
        /// Where 條件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        ISqlRawExecuteBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate);
    }
}