namespace FluentValidator.Parse;

public static class Parse<TEntity>
{
    public static Seed<TEntity> This(TEntity value) => new Seed<TEntity>(value);
}
