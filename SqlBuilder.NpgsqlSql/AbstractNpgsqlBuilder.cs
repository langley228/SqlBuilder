using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SqlBuilder.NpgsqlSql
{
    internal abstract class AbstractNpgsqlBuilder<TModel> :
        AbstractNpgsqlBuilder where TModel : class
    {
        protected AbstractNpgsqlBuilder(AbstractNpgsqlBuilder builder) : base(builder, typeof(TModel))
        {
        }

        protected AbstractNpgsqlBuilder(
            DbContext dbContext) : base(dbContext, typeof(TModel))
        {
        }
    }

    internal abstract class AbstractNpgsqlBuilder
    {
        protected StringBuilder _sb = new StringBuilder();
        protected readonly DbContext _dbContext;
        protected readonly IEntityType _entityType;
        protected List<object> _parameters = new List<object>();
        public AbstractNpgsqlBuilder(AbstractNpgsqlBuilder builder)
        {
            _sb = builder._sb;
            _dbContext = builder._dbContext;
            _parameters = builder._parameters;
            _entityType = builder._entityType;
        }

        public AbstractNpgsqlBuilder(
            AbstractNpgsqlBuilder builder,
            Type entityType)
        {
            _sb = builder._sb;
            _dbContext = builder._dbContext;
            _parameters = builder._parameters;
            _entityType = _dbContext.Model.FindEntityType(entityType);
        }

        public AbstractNpgsqlBuilder(
            DbContext dbContext,
            Type entityType
            )
        {
            _dbContext = dbContext;
            _entityType = _dbContext.Model.FindEntityType(entityType);
        }

        public List<object> Parameters
        {
            get { return _parameters; }
        }

        public string ToSql()
        {
            int i = 0;
            string sql = string.Join(Environment.NewLine, _parameters.Select(p => $"-- {{{i++}}} : {p}"));
            sql = $"{sql} {Environment.NewLine}{_sb.ToString()}";
            return sql;
        }

        protected string GetTableName()
        {
            return _entityType.GetTableName();
        }

        protected string GetColumnName(string propertyName)
        {
            var property = _entityType?.GetProperty(propertyName);
            return property.GetColumnBaseName();
        }

        protected string GetExpressionWhere(Expression expression)
        {
            if (expression is UnaryExpression)
            {
                //表示有一元 (Unary) 運算子的運算式
                UnaryExpression ue = expression as UnaryExpression;
                if (expression.NodeType == ExpressionType.Not)
                    return $" NOT ( {GetExpressionWhere(ue.Operand)} )";
            }
            else if (expression is BinaryExpression)
            {
                //還是比較 And OR
                string oper, left, right;
                BinaryExpression binaryExpression = expression as BinaryExpression;
                left = this.GetExpressionWhere(binaryExpression.Left as Expression);
                right = this.GetExpressionWhere(binaryExpression.Right as Expression);

                //=NULL 換成 IS NULL !=NULL 換成 IS NOT NULL
                if (expression.NodeType == ExpressionType.Equal && right == "NULL")
                {
                    oper = " IS ";
                }
                else if (expression.NodeType == ExpressionType.NotEqual && right == "NULL")
                {
                    oper = " IS NOT ";
                }
                else
                {
                    oper = GetOperator(expression.NodeType);
                }
                return string.Format("({0}{1}{2})", left, oper, right);
            }
            else if (expression is MethodCallExpression)
            {
                var mc = expression as MethodCallExpression;
                if (mc.Method.Name == "Contains" && mc.Arguments.Count == 1)
                {
                    return string.Format("( {0} IN ( {1} ) )",
                        GetExpressionWhere(mc.Arguments[0]),
                        GetExpressionWhere(mc.Object));
                }
            }

            if (TryParseExpressionColumnName(expression, out string name))
                return name;
            return GetExpressionValue(expression);
        }

        protected string GetExpressionValue(Expression expression)
        {
            if (expression is ConstantExpression)
            {
                //直接設值的Expression
                var ce = expression as ConstantExpression;
                return GetParameterValue(ce.Value);
            }
            else if (expression is UnaryExpression)
            {
                //表示有一元 (Unary) 運算子的運算式
                UnaryExpression ue = expression as UnaryExpression;

                if (ue.Operand is MemberExpression)
                {
                    MemberExpression me = ue.Operand as MemberExpression;
                    //取屬性值
                    return GetParameterValue(Expression.Lambda(me).Compile().DynamicInvoke());
                }
                else
                {
                    return GetParameterValue(Expression.Lambda(ue.Operand).Compile().DynamicInvoke());
                }
            }
            return GetParameterValue(Expression.Lambda(expression).Compile().DynamicInvoke());
        }

        protected string GetParameterValue(object value)
        {
            if (value is IEnumerable)
            {
                var values = value as IEnumerable;
                List<string> parameterList = new List<string>();
                foreach (var item in values)
                {
                    parameterList.Add(GetParameterValue(item));
                }
                return string.Join(",", parameterList);
            }
            if (value == null)
                return $"NULL";
            this._parameters.Add(value);
            return $"{{{_parameters.Count - 1}}}";
        }

        /// <summary>
        /// 取得操作子
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Subtract:
                    return " - ";
                case ExpressionType.Add:
                    return " + ";
                default:
                    throw new ArgumentException("不支援的Where操作");
            }
        }

        /// <summary>
        /// expression 是否能回傳欄位名稱
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="name">欄位名稱</param>
        /// <returns></returns>
        protected bool TryParseExpressionColumnName(
            Expression expression,
            out string name)
        {
            if (expression is UnaryExpression)
            {
                //針對列舉欄位名稱做修正
                //表示有一元 (Unary) 運算子的運算式
                UnaryExpression ue = expression as UnaryExpression;
                if (ue.Operand is MemberExpression)
                {
                    MemberExpression me = ue.Operand as MemberExpression;
                    if (me.Expression is ParameterExpression)
                    {
                        //欄位
                        name = GetColumnName(me.Member.Name);
                        return true;
                    }
                }
            }
            else if (expression is MemberExpression)
            {
                MemberExpression memberExpression = expression as MemberExpression;
                if (memberExpression.Expression is ParameterExpression)
                {
                    //欄位
                    name = GetColumnName(memberExpression.Member.Name);
                    return true;
                }
            }
            name = null;
            return false;
        }

        /// <summary>
        /// 取得欄位名稱
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected string ParseExpressionColumnName(Expression expression)
        {
            if (TryParseExpressionColumnName(expression, out string name))
                return name;
            throw new ArgumentException("無法轉換欄位名稱");
        }
    }
}
