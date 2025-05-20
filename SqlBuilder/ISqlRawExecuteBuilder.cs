using System.Threading;
using System.Threading.Tasks;

namespace SqlBuilder
{
    public interface ISqlRawExecuteBuilder<TModel> :
        ISqlRawBuilder<TModel>,
        ISqlRawWhereBuilder<TModel>
        where TModel : class
    {
        /// <summary>
        /// Delete 起手式
        /// </summary>
        /// <returns></returns>
        ISqlRawDeleteBuilder<TModel> SqlRawFoDelete();

        /// <summary>
        /// Update 起手式
        /// </summary>
        /// <returns></returns>
        ISqlRawUpdateBuilder<TModel> SqlRawForUpdate();

        /// <summary>
        /// Delete 起手式
        /// </summary>
        /// <typeparam name="TModel2"></typeparam>
        /// <returns></returns>
        ISqlRawDeleteBuilder<TModel2> SqlRawFoDelete<TModel2>()
            where TModel2 : class;

        /// <summary>
        /// Update 起手式
        /// </summary>
        /// <typeparam name="TModel2"></typeparam>
        /// <returns></returns>
        ISqlRawUpdateBuilder<TModel2> SqlRawForUpdate<TModel2>()
            where TModel2 : class;

        /// <summary>
        /// 執行 Sql , 建議要包交易
        /// </summary>
        /// <returns></returns>
        int ExecuteSqlRaw();

        /// <summary>
        /// 執行 Sql , 建議要包交易
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> ExecuteSqlRawAsync(
            CancellationToken cancellationToken = default);
    }
}