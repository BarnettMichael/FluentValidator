namespace FluentValidator.Validate;
/// <summary>
/// Allows generic building of fluent validation chains. 
/// Can only be run in series so as soon as the first check fails then no further checks will run
/// </summary>
public static class Check
{
    public static Seed<TEntity> That<TEntity>(TEntity value) => 
        new (value ?? throw new ArgumentNullException(nameof(value)));
}