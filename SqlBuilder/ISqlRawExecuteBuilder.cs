using System.Threading;
using System.Threading.Tasks;

namespace SqlBuilder
{
    /// <summary>
    /// 定義可執行 SQL 語句的建構器介面，提供建構與執行 DELETE、UPDATE 等 SQL 操作的方法。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    public interface ISqlRawExecuteBuilder<TModel> :
        ISqlRawBuilder<TModel>,
        ISqlRawWhereBuilder<TModel>
        where TModel : class
    {
        /// <summary>
        /// 建立 DELETE SQL 語句的建構器（針對 TModel）。
        /// </summary>
        /// <returns>DELETE 語句建構器。</returns>
        ISqlRawDeleteBuilder<TModel> SqlRawFoDelete();

        /// <summary>
        /// 建立 UPDATE SQL 語句的建構器（針對 TModel）。
        /// </summary>
        /// <returns>UPDATE 語句建構器。</returns>
        ISqlRawUpdateBuilder<TModel> SqlRawForUpdate();

        /// <summary>
        /// 建立 DELETE SQL 語句的建構器（針對指定模型類型）。
        /// </summary>
        /// <typeparam name="TModel2">指定的資料模型類型。</typeparam>
        /// <returns>DELETE 語句建構器。</returns>
        ISqlRawDeleteBuilder<TModel2> SqlRawFoDelete<TModel2>()
            where TModel2 : class;

        /// <summary>
        /// 建立 UPDATE SQL 語句的建構器（針對指定模型類型）。
        /// </summary>
        /// <typeparam name="TModel2">指定的資料模型類型。</typeparam>
        /// <returns>UPDATE 語句建構器。</returns>
        ISqlRawUpdateBuilder<TModel2> SqlRawForUpdate<TModel2>()
            where TModel2 : class;

        /// <summary>
        /// 執行 SQL 語句，建議包在交易中。
        /// </summary>
        /// <returns>受影響的資料列數。</returns>
        int ExecuteSqlRaw();

        /// <summary>
        /// 非同步執行 SQL 語句，建議包在交易中。
        /// </summary>
        /// <param name="cancellationToken">取消作業的通知。</param>
        /// <returns>受影響的資料列數。</returns>
        Task<int> ExecuteSqlRawAsync(
            CancellationToken cancellationToken = default);
    }
}