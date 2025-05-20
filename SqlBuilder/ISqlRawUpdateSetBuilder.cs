namespace SqlBuilder
{
    public interface ISqlRawUpdateSetBuilder<TModel> :
        ISqlRawUpdateBuilder<TModel>,
        ISqlRawWhereBuilder<TModel>
        where TModel : class
    {
    }
}