namespace SqlBuilder
{
    public interface ISqlRawDeleteBuilder<TModel> :
        ISqlRawBuilder<TModel>,
        ISqlRawWhereBuilder<TModel>
        where TModel : class
    {
    }
}
