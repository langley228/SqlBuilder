using System.Collections.Generic;

namespace SqlBuilder
{
    /// <summary>
    /// 定義 SQL 原始語句建構器介面，提供參數集合與產生 SQL 字串的方法。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    public interface ISqlRawBuilder<TModel> where TModel : class
    {
        /// <summary>
        /// 取得 SQL 語句所需的參數集合。
        /// </summary>
        List<object> Parameters { get; }

        /// <summary>
        /// 產生對應的 SQL 語句字串。
        /// </summary>
        /// <returns>SQL 語句。</returns>
        string ToSql();
    }
}