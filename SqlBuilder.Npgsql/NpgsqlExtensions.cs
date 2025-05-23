using Microsoft.EntityFrameworkCore;

namespace SqlBuilder.Npgsql
{
    /// <summary>
    /// 提供 DbContext 擴充方法，用於建立 SQL 刪除與更新語句的建構器。
    /// </summary>
    public static class NpgsqlExtensions
    {
        /// <summary>
        /// 建立針對指定模型類型的 DELETE SQL 語句建構器。
        /// </summary>
        /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
        /// <param name="context">EF Core 的 DbContext 實例。</param>
        /// <returns>DELETE 語句建構器。</returns>
        public static ISqlRawDeleteBuilder<TModel> SqlRawFoDelete<TModel>(this DbContext context)
            where TModel : class
        {
            return new NpgsqlExecuteBuilder<TModel>(context).SqlRawFoDelete();
        }

        /// <summary>
        /// 建立針對指定模型類型的 UPDATE SQL 語句建構器。
        /// </summary>
        /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
        /// <param name="context">EF Core 的 DbContext 實例。</param>
        /// <returns>UPDATE 語句建構器。</returns>
        public static ISqlRawUpdateBuilder<TModel> SqlRawForUpdate<TModel>(this DbContext context)
            where TModel : class
        {
            return new NpgsqlExecuteBuilder<TModel>(context).SqlRawForUpdate();
        }
    }
}
