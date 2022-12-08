namespace FluentValidator.Validate;

/// <summary>
/// Extension methods on the <see cref="Result{TEntity}"/> and <see cref="Result{TEntity, TError}"/> classes.
/// Used to enable further chaining of validation rules.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Allows additional chaining of validation methods onto a prexisting validation result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the value being passed to all the checks.</typeparam>
    /// <param name="r">The current <see cref="Result{TEntity}"/> that is having methods chained from it.</param>
    /// <param name="predicate">The validation function to pass the value into.</param>
    /// <param name="error">String that will be returned as the Error property on the returned result in the case that the predicate returns false.</param>
    /// <returns>A <see cref="Result{TEntity}"/> the state of which can be inspected to determine the success of the predicate passed as a parameter.</returns>
    public static Result<TEntity> And<TEntity>(this Result<TEntity> r, Func<TEntity, bool> predicate, string error = "")
        => r with { IsSuccess = r.IsSuccess && predicate(r.Value), Error = error };

    /// <summary>
    /// Allows additional chaining of validation methods onto a prexisting validation result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the value being passed to all the checks.</typeparam>
    /// <typeparam name="TError">The type of the error property on the returned Result object. </typeparam>
    /// <param name="r">The current <see cref="Result{TEntity, TError}"/> that is having methods chained from it.</param>
    /// <param name="predicate">The validation function to pass the value into.</param>
    /// <param name="error">value that will be returned as the Error property on the returned result in the case that the predicate returns false.</param>
    /// <returns>A <see cref="Result{TEntity, TError}"/> the state of which can be inspected to determine the success of the predicates passed as parameters.</returns>
    public static Result<TEntity, TError> And<TEntity, TError>(
        this Result<TEntity, TError> r, Func<TEntity, bool> predicate, TError error)
    {
        if (r.IsSuccess && predicate(r.Value))
        {
            return r;
        }
        return r with { IsSuccess = false, Error = error };
    }

    /// <summary>
    /// Allows additional chaining of validation methods onto a prexisting validation result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the value being passed to all the checks.</typeparam>
    /// <typeparam name="TError">The type of the error property on the returned Result object. </typeparam>
    /// <param name="r">The current <see cref="Result{TEntity, TError}"/> that is having methods chained from it.</param>
    /// <param name="predicate">The validation function to pass the value into.</param>
    /// <param name="errorGenerator">A function that takes a vlaue of TEntity as a parameter and returns an instance of TError. The result of this function
    /// will be set as the Error property of the returned Result if the predicate returns false.</param>
    /// <returns>A <see cref="Result{TEntity, TError}"/> the state of which can be inspected to determine the success of the predicates passed as parameters.</returns>
    public static Result<TEntity, TError> And<TEntity, TError>(
        this Result<TEntity, TError> r, Func<TEntity, bool> predicate, Func<TEntity, TError> errorGenerator)
    {
        if (r.IsSuccess && predicate(r.Value))
        {
            return r;
        }
        return r with { IsSuccess = false, Error = errorGenerator(r.Value) };
    }

    /// <summary>
    /// Allows additional chaining of an array of validation methods onto a prexisting validation result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the value being passed to all the checks.</typeparam>
    /// <param name="r">The current <see cref="Result{TEntity}"/> that is having methods chained from it.</param>
    /// <param name="checks">An array of predicates, and their associated error value combined into tuples. 
    /// The value contained within the prexisting result will be passed to each one sequentially until one of the
    /// predicates returns false, or they have all returned true.
    /// </param>
    /// <returns>A <see cref="Result{TEntity}"/> the state of which can be inspected to determine the success of the predicate passed as a parameter.</returns>
    public static Result<TEntity> AndAll<TEntity>(this Result<TEntity> r, params Func<TEntity, bool>[] checks)
    {
        if (checks.Length == 0) { return r; }

        var currentResult = r;
        for (var i = 0; i < checks.Length && currentResult.IsSuccess; i++)
        {
            currentResult = currentResult.And(checks[i]);
        }

        return currentResult;
    }

    /// <summary>
    /// Allows additional chaining of an array of validation methods onto a prexisting validation result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the value being passed to all the checks.</typeparam>
    /// <typeparam name="TError">The type of the error property on the returned Result object. </typeparam>
    /// <param name="r">The current <see cref="Result{TEntity, TError}"/> that is having methods chained from it.</param>
    /// <param name="checks">An array of predicates, and their associated error value combined into tuples. 
    /// The value contained within the prexisting result will be passed to each one sequentially until one of the
    /// predicates returns false, or they have all returned true.
    /// </param>
    /// <returns>A <see cref="Result{TEntity, TError}"/> the state of which can be inspected to determine the success of the predicates passed as parameters.</returns>
    public static Result<TEntity, TError> AndAll<TEntity, TError>(this Result<TEntity, TError> r,
        params (Func<TEntity, bool> predicate, TError error)[] checks)
    {
        if (checks.Length == 0) { return r; }

        var currentResult = r;
        for (var i = 0; i < checks.Length && currentResult.IsSuccess; i++)
        {
            currentResult = currentResult.And(checks[i].predicate, checks[i].error);
        }

        return currentResult;
    }

    /// <summary>
    /// Allows additional chaining of an array of validation methods onto a prexisting validation result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the value being passed to all the checks.</typeparam>
    /// <typeparam name="TError">The type of the error property on the returned Result object. </typeparam>
    /// <param name="r">The current <see cref="Result{TEntity, TError}"/> that is having methods chained from it.</param>
    /// <param name="checks">An array of predicates, and their associated errorGenerator functions combined into tuples. 
    /// The value contained within the prexisting result will be passed to each one sequentially until one of the
    /// predicates returns false, or they have all returned true.
    /// </param>
    /// <returns>A <see cref="Result{TEntity, TError}"/> the state of which can be inspected to determine the success of the predicates passed as parameters.</returns>
        public static Result<TEntity, TError> AndAll<TEntity, TError>(this Result<TEntity, TError> r,
            params (Func<TEntity, bool> predicate, Func<TEntity, TError> errorGenerator)[] checks)
    {
        if (checks.Length == 0) { return r; }

        var currentResult = r;
        for (var i = 0; i < checks.Length && currentResult.IsSuccess; i++)
        {
            currentResult = currentResult.And(checks[i].predicate, checks[i].errorGenerator);
        }

        return currentResult;
    }
}
