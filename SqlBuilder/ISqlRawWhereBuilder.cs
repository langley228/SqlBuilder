using System;
using System.Linq.Expressions;

namespace SqlBuilder
{
    /// <summary>
    /// 定義 SQL Where 條件建構器介面，提供設定查詢條件的方法。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    public interface ISqlRawWhereBuilder<TModel> where TModel : class
    {
        /// <summary>
        /// 設定 Where 條件。
        /// </summary>
        /// <param name="predicate">條件運算式。</param>
        /// <returns>回傳可執行 SQL 的建構器。</returns>
        ISqlRawExecuteBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate);
    }
}