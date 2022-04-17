namespace FluentValidator.Validate;
/// <summary>
/// Allows generic building of fluent validation chains. 
/// Can only be run in serial so as soon as the first check fails then no further checks will run
/// </summary>
public static class Check
{
    public static Seed<TEntity> That<TEntity>(TEntity value) => new Seed<TEntity>(value);
}