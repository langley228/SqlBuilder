using System.Collections.Generic;

namespace SqlBuilder
{
    public interface ISqlRawBuilder<TModel> where TModel : class
    {
        /// <summary>
        /// 
        /// </summary>
        List<object> Parameters { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string ToSql();
    }
}