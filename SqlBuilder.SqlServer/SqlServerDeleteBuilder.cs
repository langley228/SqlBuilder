using System;
using System.Linq.Expressions;

namespace SqlBuilder.SqlServer
{
    /// <summary>
    /// Npgsql DELETE SQL 語句建構器，實作 ISqlRawDeleteBuilder 介面，提供 WHERE 條件設定與建構功能。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    internal class SqlServerDeleteBuilder<TModel> :
        AbstractSqlServerBuilder<TModel>,
        ISqlRawDeleteBuilder<TModel> where TModel : class
    {
        /// <summary>
        /// 以現有建構器初始化 DELETE 語句建構器。
        /// </summary>
        /// <param name="builder">抽象建構器實例。</param>
        public SqlServerDeleteBuilder(AbstractSqlServerBuilder<TModel> builder) : base(builder)
        {
        }

        /// <summary>
        /// 設定 WHERE 條件，並回傳可執行 SQL 的建構器。
        /// </summary>
        /// <param name="predicate">條件運算式。</param>
        /// <returns>可執行 SQL 的建構器。</returns>
        public ISqlRawExecuteBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate)
        {
            _sb.AppendLine($" WHERE ")
               .AppendLine(GetExpressionWhere(predicate.Body));
            return new SqlServerExecuteBuilder<TModel>(this);
        }
    }
}