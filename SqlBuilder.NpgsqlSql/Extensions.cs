using Microsoft.EntityFrameworkCore;

namespace SqlBuilder.NpgsqlSql
{
    public static class NpgsqlSqlExtensions
    {
        public static ISqlRawDeleteBuilder<TModel> SqlRawFoDelete<TModel>(this DbContext context)
            where TModel : class
        {
            return new NpgsqlExecuteBuilder<TModel>(context).SqlRawFoDelete();
        }

        public static ISqlRawUpdateBuilder<TModel> SqlRawForUpdate<TModel>(this DbContext context)
            where TModel : class
        {
            return new NpgsqlExecuteBuilder<TModel>(context).SqlRawForUpdate();
        }
    }
}
