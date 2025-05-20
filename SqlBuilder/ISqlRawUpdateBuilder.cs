using System;
using System.Linq.Expressions;

namespace SqlBuilder
{
    public interface ISqlRawUpdateBuilder<TModel> :
        ISqlRawBuilder<TModel>
        where TModel : class
    {
        /// <summary>
        /// 單一欄位值累加
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlRawUpdateSetBuilder<TModel> Inc<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value);

        /// <summary>
        /// 設定單一欄位值
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISqlRawUpdateSetBuilder<TModel> Set<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            TProperty value);

        /// <summary>
        /// 依次設定多欄位值
        /// </summary>
        /// <typeparam name="TSet"></typeparam>
        /// <param name="setValue"></param>
        /// <returns></returns>
        ISqlRawUpdateSetBuilder<TModel> Set<TSet>(TSet setValue) where TSet : class;
    }
}