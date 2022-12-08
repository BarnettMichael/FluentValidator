namespace FluentValidator.Validate;

/// <summary>
/// Extension methods that extend the <see cref="Seed{TEntity}"/> class to enable various ways of starting
/// a validation chain.
/// </summary>
public static class SeedExtensions
{
    /// <summary>
    /// Pass the value into a function provided, will return a result based on whether that function returns true or false.
    /// If the function returns true then the result will include the value that can then be chained for further validation.
    /// If the function returns false then the result IsSuccess property will be false and the Error property can optionally
    /// be set to a custom error string if passed in as a parameter.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="s"></param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<TEntity> Is<TEntity>(this Seed<TEntity> s, Func<TEntity, bool> predicate, string error = "") 
        => new(s.Value, predicate(s.Value), error);

    // TODO write tests around this and see if it can actually work as a way of accepting more parameters.
    // Look at https://stackoverflow.com/questions/22834120/func-with-unknown-number-of-parameters to see if that is helpful.
    // Is it better to use Delegate.DynamicInvoke instead?
    // Or better to do worked examples of currying to avoid needing this method in the first place.
    // Don't forget to replicate for other error cases as well 
    //public static Result<TEntity> Is<TEntity, TParam>(this Seed<TEntity> s, Func<TEntity, TParam, bool> predicate, string error = "", params object[] parameters)
    //{
    //    if (parameters.Length != 1)
    //    {
    //        throw new ArgumentException($"Predicate function is expecting two parameters including {typeof(TEntity)}, incorrect number of parameters provided");
    //    }
    //    var param = (TParam)parameters.Single();
    //    return new(s.Value, predicate(s.Value, param), error);
    //}

    /// <summary>
    /// Pass the value into a function provided, will return a result based on whether that function returns true or false.
    /// If the function returns true then the result will include the value that can then be chained for further validation.
    /// The value stored in the error property will be the default value for the type of TError
    /// If the function returns false then the result IsSuccess property will be false and the Error property will be set
    /// to the error value that is passed as a parameter.
    /// </summary>
    /// <typeparam name="TEntity">Type of the value that is going to be passed into predicate function.</typeparam>
    /// <typeparam name="TError">Type of the error property in the methods result value.</typeparam>
    /// <param name="s">The start of the</param>
    /// <param name="predicate"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<TEntity, TError> Is<TEntity, TError>(this Seed<TEntity> s, Func<TEntity, bool> predicate, TError error)
    {
        if (predicate(s.Value))
        {
            // This is setting the error to be the default value of the TError because currently unable to 
            // assign null. See https://github.com/dotnet/roslyn/issues/53139 for details on why this is not
            // possible. The only way this could be avoided would be to maintain two instances of Result<T,E>
            // one where E : struct and one where E : class. This would lock each chain into specific types
            // of errors throughout. Maybe some future UnionType within C# could then allow the two types
            // to be combined back into one.
            return new(s.Value, IsSuccess: true, Error: default);
        }
        return new(s.Value, IsSuccess: false, error);
    }

    /// <summary>
    /// Pass the value into a function provided, will return a result based on whether that function returns true or false.
    /// If the function returns true then the result will include the value that can then be chained for further validation and
    /// the value stored in the error property will be the default value for the type of TError
    /// If the function returns false then the result IsSuccess property will be false and the errorGenerator function will be
    /// called to generate a custom error which is stored in the Result.Error property.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <param name="s"></param>
    /// <param name="predicate"></param>
    /// <param name="errorGenerator"></param>
    /// <returns></returns>
    public static Result<TEntity, TError> Is<TEntity, TError>(this Seed<TEntity> s, Func<TEntity, bool> predicate, Func<TEntity, TError> errorGenerator) 
        => predicate(s.Value) ? (new(s.Value, IsSuccess: true, Error: default))
                              : (new(s.Value, IsSuccess: false, errorGenerator(s.Value)));

    /// <summary>
    /// Pass the value into each check function in turn, only continuing if all of the previous checks have returned true.
    /// </summary>
    /// <typeparam name="TEntity">The type of the value being checked</typeparam>
    /// <param name="s">The initial object to build a validation chain from stores the value being checked</param>
    /// <param name="checks">An enumerable collection of checks that will have the value passed in as a parameter in turn</param>
    /// <returns>A result object that declares whether or not all of the checks were successful.
    /// The results error value will always be an empty string as no error declaration is input.
    /// </returns>
    public static Result<TEntity> IsAll<TEntity>(this Seed<TEntity> s, params Func<TEntity, bool>[] checks)
    {
        if (checks.Length == 0) { return new(s.Value, true); }
        var firstCheck = checks[0];
        var currentResult = new Result<TEntity>(s.Value, firstCheck(s.Value));
        if (checks.Length == 1) { return currentResult; }

        for (var i = 1; currentResult.IsSuccess && i < checks.Length; i++)
        {
            currentResult = currentResult.And(checks[i]);
        }

        return currentResult;
    }

    /// <summary>
    /// Passes the value being validated into each of the predicate checks in turn. The first one to fail will end the validation chain.
    /// </summary>
    /// <typeparam name="TEntity">Type of the value being validated</typeparam>
    /// <typeparam name="TError">Type of the error result returned by the validation</typeparam>
    /// <param name="s">Seed that contains the value being validated</param>
    /// <param name="checks">An array of tuples, the first item of the tuple being a function to validate the value, 
    /// and the second item being the error to return if the validation fails.</param>
    /// <returns>A result object that declares whether or not all of the checks were successful.
    /// </returns>
    public static Result<TEntity, TError> IsAll<TEntity, TError>(this Seed<TEntity> s, params (Func<TEntity, bool> predicate, TError error)[] checks)
    {
        if (checks.Length == 0) { return new(s.Value, true, default); }
        Result<TEntity, TError> currentResult;

        var firstCheck = checks[0];
        if (firstCheck.predicate(s.Value))
        {
            currentResult = new(s.Value, true, default);
        }
        else
        {
            currentResult = new(s.Value, false, firstCheck.error);
        }

        if (checks.Length == 1) { return currentResult; }

        for (var i = 1; currentResult.IsSuccess && i < checks.Length; i++)
        {
            currentResult = currentResult.And(checks[i].predicate, checks[i].error);
        }

        return currentResult;
    }

    /// <summary>
    /// Passes the value being validated into each of the predicate checks in turn. The first one to fail will end the validation chain.
    /// </summary>
    /// <typeparam name="TEntity">Type of the value being validated</typeparam>
    /// <typeparam name="TError">Type of the error result returned by the validation</typeparam>
    /// <param name="s">Seed that contains the value being validated</param>
    /// <param name="checks">An array of tuples, the first item of the tuple being a function to validate the value, 
    /// and the second item being a function that takes the value as a parameter and returns the error for the whole function to return if the validation fails.</param>
    /// <returns>A result object that declares whether or not all of the checks were successful.
    /// </returns>
    public static Result<TEntity, TError> IsAll<TEntity, TError>(this Seed<TEntity> s, params (Func<TEntity, bool> predicate, Func<TEntity, TError> errorGenerator)[] checks)
    {
        if (checks.Length == 0) { return new(s.Value, true, default); }
        Result<TEntity, TError> currentResult;

        var firstCheck = checks[0];
        if (firstCheck.predicate(s.Value))
        {
            currentResult = new(s.Value, true, default);
        }
        else
        {
            currentResult = new(s.Value, false, firstCheck.errorGenerator(s.Value));
        }

        if (checks.Length == 1) { return currentResult; }

        for (var i = 1; currentResult.IsSuccess && i < checks.Length; i++)
        {
            currentResult = currentResult.And(checks[i].predicate, checks[i].errorGenerator);
        }

        return currentResult;
    }
}
