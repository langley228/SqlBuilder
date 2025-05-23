using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SqlBuilder.Npgsql
{
    /// <summary>
    /// Npgsql 可執行 SQL 語句建構器，實作 ISqlRawExecuteBuilder 介面，提供 DELETE、UPDATE 語句建構與執行功能。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    internal class NpgsqlExecuteBuilder<TModel> : AbstractNpgsqlBuilder<TModel>, ISqlRawExecuteBuilder<TModel> where TModel : class
    {
        /// <summary>
        /// 以現有建構器初始化執行建構器。
        /// </summary>
        /// <param name="builder">抽象建構器實例。</param>
        public NpgsqlExecuteBuilder(AbstractNpgsqlBuilder builder) : base(builder)
        {
        }

        /// <summary>
        /// 以 DbContext 初始化執行建構器。
        /// </summary>
        /// <param name="dbContext">EF Core 的 DbContext 實例。</param>
        public NpgsqlExecuteBuilder(DbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 建立 DELETE SQL 語句建構器（針對 TModel）。
        /// </summary>
        /// <returns>DELETE 語句建構器。</returns>
        public ISqlRawDeleteBuilder<TModel> SqlRawFoDelete()
        {
            if (_sb.Length > 0)
                _sb.AppendLine(";");
            _sb.AppendLine($"DELETE FROM {GetTableName()} ");
            return new NpgsqlDeleteBuilder<TModel>(this);
        }

        /// <summary>
        /// 建立 UPDATE SQL 語句建構器（針對 TModel）。
        /// </summary>
        /// <returns>UPDATE 語句建構器。</returns>
        public ISqlRawUpdateBuilder<TModel> SqlRawForUpdate()
        {
            if (_sb.Length > 0)
                _sb.AppendLine(";");
            _sb.AppendLine($"UPDATE {GetTableName()} ");
            return new NpgsqlUpdateBuilder<TModel>(this);
        }

        /// <summary>
        /// 建立 DELETE SQL 語句建構器（針對指定模型類型）。
        /// </summary>
        /// <typeparam name="TModel2">指定的資料模型類型。</typeparam>
        /// <returns>DELETE 語句建構器。</returns>
        public ISqlRawDeleteBuilder<TModel2> SqlRawFoDelete<TModel2>() where TModel2 : class
        {
            return new NpgsqlExecuteBuilder<TModel2>(this).SqlRawFoDelete();
        }

        /// <summary>
        /// 建立 UPDATE SQL 語句建構器（針對指定模型類型）。
        /// </summary>
        /// <typeparam name="TModel2">指定的資料模型類型。</typeparam>
        /// <returns>UPDATE 語句建構器。</returns>
        public ISqlRawUpdateBuilder<TModel2> SqlRawForUpdate<TModel2>() where TModel2 : class
        {
            return new NpgsqlExecuteBuilder<TModel2>(this).SqlRawForUpdate();
        }

        /// <summary>
        /// 設定 AND 條件，並回傳自身以便串接。
        /// </summary>
        /// <param name="predicate">條件運算式。</param>
        /// <returns>自身建構器。</returns>
        public ISqlRawExecuteBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate)
        {
            _sb.AppendLine($" AND (")
               .AppendLine(GetExpressionWhere(predicate.Body))
               .AppendLine($" ) ");
            return this;
        }

        /// <summary>
        /// 執行 SQL 語句，建議包在交易中。
        /// </summary>
        /// <returns>受影響的資料列數。</returns>
        public int ExecuteSqlRaw()
        {
            if (_sb.Length > 0)
                _sb.AppendLine(";");
            return _dbContext.Database
                .ExecuteSqlRaw(_sb.ToString(), _parameters);
        }

        /// <summary>
        /// 非同步執行 SQL 語句，建議包在交易中。
        /// </summary>
        /// <param name="cancellationToken">取消作業的通知。</param>
        /// <returns>受影響的資料列數。</returns>
        public Task<int> ExecuteSqlRawAsync(
            CancellationToken cancellationToken = default)
        {
            if (_sb.Length > 0)
                _sb.AppendLine(";");
            return _dbContext.Database
                .ExecuteSqlRawAsync(_sb.ToString(), _parameters, cancellationToken);
        }
    }
}
