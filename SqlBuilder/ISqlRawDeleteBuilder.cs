namespace SqlBuilder
{
    /// <summary>
    /// 定義 SQL 刪除語句建構器介面，繼承自 ISqlRawBuilder 及 ISqlRawWhereBuilder，
    /// 用於建構帶有條件的 DELETE SQL 語句。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    public interface ISqlRawDeleteBuilder<TModel> :
        ISqlRawBuilder<TModel>,
        ISqlRawWhereBuilder<TModel>
        where TModel : class
    {
    }
}
