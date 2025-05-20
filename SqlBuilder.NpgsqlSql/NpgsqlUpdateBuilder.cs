using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlBuilder.NpgsqlSql
{
    internal class NpgsqlUpdateBuilder<TModel> :
        AbstractNpgsqlBuilder<TModel>,
        ISqlRawUpdateBuilder<TModel> where TModel : class
    {
        public NpgsqlUpdateBuilder(AbstractNpgsqlBuilder<TModel> builder) : base(builder)
        {
        }

        public ISqlRawUpdateSetBuilder<TModel> Inc<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value)
        {
            int index = this._parameters.Count;
            var name = ParseExpressionColumnName(expression.Body);
            _parameters.Add(value);
            _sb.Append($" SET {name}+({{{index}}}) ");
            return new NpgsqlUpdateSetBuilder<TModel>(this);
        }

        public ISqlRawUpdateSetBuilder<TModel> Set<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value)
        {
            var name = ParseExpressionColumnName(expression.Body);
            string paramerter = GetParameterValue(value);
            _sb.Append($" SET {name}={paramerter} ");
            return new NpgsqlUpdateSetBuilder<TModel>(this);
        }

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
            return new NpgsqlUpdateSetBuilder<TModel>(this);
        }
    }
}