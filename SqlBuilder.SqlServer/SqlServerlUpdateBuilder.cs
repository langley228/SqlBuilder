using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlBuilder.SqlServer
{
    /// <summary>
    /// Npgsql UPDATE SQL 語句建構器，實作 ISqlRawUpdateBuilder 介面，提供 SET 與欄位累加等功能。
    /// </summary>
    /// <typeparam name="TModel">資料模型類型，必須為參考型別。</typeparam>
    internal class SqlServerlUpdateBuilder<TModel> :
        AbstractSqlServerBuilder<TModel>,
        ISqlRawUpdateBuilder<TModel> where TModel : class
    {
        /// <summary>
        /// 以現有建構器初始化 UPDATE 語句建構器。
        /// </summary>
        /// <param name="builder">抽象建構器實例。</param>
        public SqlServerlUpdateBuilder(AbstractSqlServerBuilder<TModel> builder) : base(builder)
        {
        }

        /// <summary>
        /// 單一欄位值累加。
        /// </summary>
        /// <typeparam name="TProperty">欄位型別。</typeparam>
        /// <param name="expression">指定要累加的欄位運算式。</param>
        /// <param name="value">累加的值。</param>
        /// <returns>回傳可設定 SET 與 WHERE 條件的建構器。</returns>
        public ISqlRawUpdateSetBuilder<TModel> Inc<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value)
        {
            int index = this._parameters.Count;
            var name = ParseExpressionColumnName(expression.Body);
            _parameters.Add(value);
            _sb.Append($" SET {name}+({{{index}}}) ");
            return new SqlServerUpdateSetBuilder<TModel>(this);
        }

        /// <summary>
        /// 設定單一欄位值。
        /// </summary>
        /// <typeparam name="TProperty">欄位型別。</typeparam>
        /// <param name="expression">指定要設定的欄位運算式。</param>
        /// <param name="value">設定的值。</param>
        /// <returns>回傳可設定 SET 與 WHERE 條件的建構器。</returns>
        public ISqlRawUpdateSetBuilder<TModel> Set<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value)
        {
            var name = ParseExpressionColumnName(expression.Body);
            string paramerter = GetParameterValue(value);
            _sb.Append($" SET {name}={paramerter} ");
            return new SqlServerUpdateSetBuilder<TModel>(this);
        }

        /// <summary>
        /// 依次設定多欄位值。
        /// </summary>
        /// <typeparam name="TSet">設定值的型別，必須為參考型別。</typeparam>
        /// <param name="setValue">包含多個欄位設定值的物件。</param>
        /// <returns>回傳可設定 SET 與 WHERE 條件的建構器。</returns>
        public ISqlRawUpdateSetBuilder<TModel> Set<TSet>(TSet setValue) where TSet : class
        {
            Type typeM = typeof(TModel);
            Type typeS = typeof(TSet);
            PropertyInfo[] propsM = typeM.GetProperties();
            PropertyInfo[] propsS = typeS.GetProperties().Where(m => propsM.Any(x => x.Name == m.Name)).ToArray();

            var columns = propsS.Select(p => new
            {
                name = GetColumnName(p.Name),
                value = GetParameterValue(p.GetValue(setValue))
            });
            _sb.Append($" SET {string.Join($", ", columns.Select(m => $"{m.name}={m.value}"))}");
            return new SqlServerUpdateSetBuilder<TModel>(this);
        }
    }
}