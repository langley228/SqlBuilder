using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SqlBuilder.SqlServer
{
    /// <summary>
    /// 泛型抽象 Npgsql SQL 建構器，繼承自 AbstractNpgsqlBuilder，提供針對指定模型類型的建構功能。
    /// </summary>
    internal abstract class AbstractSqlServerBuilder<TModel> :
        AbstractSqlServerBuilder where TModel : class
    {
        /// <summary>
        /// 以現有建構器與模型型別初始化。
        /// </summary>
        protected AbstractSqlServerBuilder(AbstractSqlServerBuilder builder) : base(builder, typeof(TModel))
        {
        }

        /// <summary>
        /// 以 DbContext 與模型型別初始化。
        /// </summary>
        protected AbstractSqlServerBuilder(
            DbContext dbContext) : base(dbContext, typeof(TModel))
        {
        }
    }

    /// <summary>
    /// 抽象 Npgsql SQL 建構器，提供 SQL 字串組合、參數管理、欄位與條件運算式解析等功能。
    /// </summary>
    internal abstract class AbstractSqlServerBuilder
    {
        /// <summary>SQL 字串組合器。</summary>
        protected StringBuilder _sb = new StringBuilder();
        /// <summary>EF Core DbContext</summary>
        protected readonly DbContext _dbContext;
        /// <summary>EF Core 實體型別</summary>
        protected readonly IEntityType _entityType;
        /// <summary>SQL 參數</summary>
        protected List<object> _parameters = new List<object>();
        public AbstractSqlServerBuilder(AbstractSqlServerBuilder builder)
        {
            _sb = builder._sb;
            _dbContext = builder._dbContext;
            _parameters = builder._parameters;
            _entityType = builder._entityType;
        }

        public AbstractSqlServerBuilder(
            AbstractSqlServerBuilder builder,
            Type entityType)
        {
            _sb = builder._sb;
            _dbContext = builder._dbContext;
            _parameters = builder._parameters;
            _entityType = _dbContext.Model.FindEntityType(entityType);
        }

        public AbstractSqlServerBuilder(
            DbContext dbContext,
            Type entityType
            )
        {
            _dbContext = dbContext;
            _entityType = _dbContext.Model.FindEntityType(entityType);
        }

        /// <summary>
        /// 取得 SQL 參數列表
        /// </summary>
        public List<object> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// 產生 SQL 字串
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            int i = 0;
            string sql = string.Join(Environment.NewLine, _parameters.Select(p => $"-- @P_{i++} : {p}"));
            sql = $"{sql} {Environment.NewLine}{_sb.ToString()}";
            return sql;
        }

        /// <summary>
        /// 取得資料表名稱
        /// </summary>
        /// <returns></returns>
        protected string GetTableName()
        {
            return _entityType.GetTableName();
        }

        /// <summary>
        /// 取得欄位名稱
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected string GetColumnName(string propertyName)
        {
            var property = _entityType?.GetProperty(propertyName);
            return property.GetColumnName();
        }

        /// <summary>
        /// 解析 Where 子句中的運算式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 取得運算式的值
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 取得參數的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string GetParameterValue(object value)
        {
            if (value is IEnumerable)
            {
                if (value is not string)
                {
                    var values = value as IEnumerable;
                    List<string> parameterList = new List<string>();
                    foreach (var item in values)
                    {
                        parameterList.Add(GetParameterValue(item));
                    }
                    return string.Join(",", parameterList);
                }
            }
            if (value == null)
                return $"NULL";
            this._parameters.Add(value);
            return $"@P_{_parameters.Count - 1}";
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
