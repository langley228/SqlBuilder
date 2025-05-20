using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlBuilder.NpgsqlSql
{
    internal class NpgsqlUpdateSetBuilder<TModel> :
        AbstractNpgsqlBuilder<TModel>,
        ISqlRawUpdateSetBuilder<TModel> where TModel : class
    {
        public NpgsqlUpdateSetBuilder(AbstractNpgsqlBuilder<TModel> builder) : base(builder)
        {
        }

        public ISqlRawUpdateSetBuilder<TModel> Inc<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value)
        {
            int index = this._parameters.Count;
            var name = ParseExpressionColumnName(expression.Body);
            _parameters.Add(value);
            _sb.Append($", {name}={name}+({{{index}}}) ");
            return this;
        }

        public ISqlRawUpdateSetBuilder<TModel> Set<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value)
        {
            int index = this._parameters.Count;
            var name = ParseExpressionColumnName(expression.Body);
            string paramerter = GetParameterValue(value);
            _sb.Append($", {name}={paramerter} ");
            return this;
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
            _sb.Append(string.Join("", columns.Select(m => $", {m.name}={m.value} ")));
            return this;
        }

        public ISqlRawExecuteBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate)
        {
            _sb.AppendLine()
               .AppendLine($" WHERE ")
               .AppendLine(GetExpressionWhere(predicate.Body));
            return new NpgsqlExecuteBuilder<TModel>(this);
        }
    }
}