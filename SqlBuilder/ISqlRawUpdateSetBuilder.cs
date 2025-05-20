namespace SqlBuilder
{
    /// <summary>
    /// 定義 SQL 更新語句建構器介面，繼承自 ISqlRawUpdateBuilder 及 ISqlRawWhereBuilder，
    /// 用於建構帶有 SET 與 WHERE 條件的 UPDATE SQL 語句。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    public interface ISqlRawUpdateSetBuilder<TModel> :
        ISqlRawUpdateBuilder<TModel>,
        ISqlRawWhereBuilder<TModel>
        where TModel : class
    {
    }
}