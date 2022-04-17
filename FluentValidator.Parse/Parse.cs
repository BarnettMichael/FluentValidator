namespace FluentValidator.Parse;

public static class Parse<TEntity>
{
    public static ParseSeed<TEntity> This(TEntity value) => new ParseSeed<TEntity>(value);
}
