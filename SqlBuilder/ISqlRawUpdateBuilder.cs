using System;
using System.Linq.Expressions;

namespace SqlBuilder
{
    /// <summary>
    /// 定義 SQL 更新語句建構器介面，提供設定欄位值與累加欄位值的方法。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    public interface ISqlRawUpdateBuilder<TModel> :
        ISqlRawBuilder<TModel>
        where TModel : class
    {
        /// <summary>
        /// 單一欄位值累加。
        /// </summary>
        /// <typeparam name="TProperty">欄位型別。</typeparam>
        /// <param name="expression">指定要累加的欄位運算式。</param>
        /// <param name="value">累加的值。</param>
        /// <returns>回傳可設定 SET 與 WHERE 條件的建構器。</returns>
        ISqlRawUpdateSetBuilder<TModel> Inc<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value);

        /// <summary>
        /// 設定單一欄位值。
        /// </summary>
        /// <typeparam name="TProperty">欄位型別。</typeparam>
        /// <param name="expression">指定要設定的欄位運算式。</param>
        /// <param name="value">設定的值。</param>
        /// <returns>回傳可設定 SET 與 WHERE 條件的建構器。</returns>
        ISqlRawUpdateSetBuilder<TModel> Set<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value);

        /// <summary>
        /// 依次設定多欄位值。
        /// </summary>
        /// <typeparam name="TSet">設定值的型別，必須為參考型別。</typeparam>
        /// <param name="setValue">包含多個欄位設定值的物件。</param>
        /// <returns>回傳可設定 SET 與 WHERE 條件的建構器。</returns>
        ISqlRawUpdateSetBuilder<TModel> Set<TSet>(TSet setValue) where TSet : class;
    }
}