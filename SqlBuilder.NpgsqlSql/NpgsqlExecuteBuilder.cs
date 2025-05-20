using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SqlBuilder.NpgsqlSql
{
    internal class NpgsqlExecuteBuilder<TModel> : AbstractNpgsqlBuilder<TModel>, ISqlRawExecuteBuilder<TModel> where TModel : class
    {
        public NpgsqlExecuteBuilder(AbstractNpgsqlBuilder builder) : base(builder)
        {
        }

        public NpgsqlExecuteBuilder(DbContext dbContext) : base(dbContext)
        {
        }

        public ISqlRawDeleteBuilder<TModel> SqlRawFoDelete()
        {
            if (_sb.Length > 0)
                _sb.AppendLine(";");
            _sb.AppendLine($"DELETE FROM {GetTableName()} ");
            return new NpgsqlDeleteBuilder<TModel>(this);
        }

        public ISqlRawUpdateBuilder<TModel> SqlRawForUpdate()
        {
            if (_sb.Length > 0)
                _sb.AppendLine(";");
            _sb.AppendLine($"UPDATE {GetTableName()} ");
            return new NpgsqlUpdateBuilder<TModel>(this);
        }

        public ISqlRawDeleteBuilder<TModel2> SqlRawFoDelete<TModel2>() where TModel2 : class
        {
            return new NpgsqlExecuteBuilder<TModel2>(this).SqlRawFoDelete();
        }

        public ISqlRawUpdateBuilder<TModel2> SqlRawForUpdate<TModel2>() where TModel2 : class
        {
            return new NpgsqlExecuteBuilder<TModel2>(this).SqlRawForUpdate();
        }

        public ISqlRawExecuteBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate)
        {
            _sb.AppendLine($" AND (")
               .AppendLine(GetExpressionWhere(predicate.Body))
               .AppendLine($" ) ");
            return this;
        }

        public int ExecuteSqlRaw()
        {
            if (_sb.Length > 0)
                _sb.AppendLine(";");
            return _dbContext.Database
                .ExecuteSqlRaw(_sb.ToString(), _parameters);
        }

        public Task<int> ExecuteSqlRawAsync(
            CancellationToken cancellationToken = default)
        {
            if (_sb.Length > 0)
                _sb.AppendLine(";");
            return _dbContext.Database
                .ExecuteSqlRawAsync(_sb.ToString(), _parameters, cancellationToken);
        }
    }
}
