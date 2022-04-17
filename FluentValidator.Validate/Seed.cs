namespace FluentValidator.Validate;

/// <summary>
/// Class that represents the start of a chain of rules.
/// </summary>
/// <typeparam name="TEntity">type of object being checked in each of the validation rules.</typeparam>
/// <param name="Value">The value that will be checked by any rule added to the chain</param>
public record Seed<TEntity>(TEntity Value); // Bad Name please fix!
