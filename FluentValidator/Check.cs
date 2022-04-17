namespace FluentValidator;
/// <summary>
/// Allows generic building of fluent validation chains. 
/// Can only be run in serial so as soon as the first check fails then no further checks will run
/// </summary>
public static class Check
{
    public static Seed<TEntity> That<TEntity>(TEntity value) => new Seed<TEntity>(value);
}

/// <summary>
/// Class that represents the start of a chain of rules.
/// </summary>
/// <typeparam name="TEntity">type of object being checked in each of the validation rules.</typeparam>
/// <param name="Value">The value that will be checked by any rule added to the chain</param>
public record Seed<TEntity>(TEntity Value); // Bad Name please fix!

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
    public static Result<TEntity> Is<TEntity>(this Seed<TEntity> s, Func<TEntity, bool> predicate, string error = "") => new(s.Value, predicate(s.Value), error);

    // TODO write tests around this and see if it can actually work as a way of accepting more parameters.
    // Look at https://stackoverflow.com/questions/22834120/func-with-unknown-number-of-parameters to see if that is helpful.
    // Is it better to use Delegate.DynamicInvoke instead?
    // Or better to do worked examples of currying to avoid needing this method in the first place.
    // Don't forget to replicate for other error cases as well 
    public static Result<TEntity> Is<TEntity, TParam>(this Seed<TEntity> s, Func<TEntity, TParam, bool> predicate, string error = "", params object[] parameters)
    {
        if (parameters.Length != 1)
        {
            throw new ArgumentException($"Predicate function is expecting two parameters including {typeof(TEntity)}, incorrect number of parameters provided");
        }
        var param = (TParam)parameters.Single();
        return new(s.Value, predicate(s.Value, param), error);
    }

    /// <summary>
    /// Pass the value into a function provided, will return a result based on whether that function returns true or false.
    /// If the function returns true then the result will include the value that can then be chained for further validation.
    /// The value stored in the error property will be the default of the TError
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
    /// If the function returns true then the result will include the value that can then be chained for further validation.
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
    {
        if (predicate(s.Value))
        {
            return new(s.Value, IsSuccess: true, Error: default);
        }
        return new(s.Value, IsSuccess: false, errorGenerator(s.Value));
    }

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

public record Result<TEntity>(TEntity Value, bool IsSuccess, string Error = "")
{
    public static implicit operator bool(Result<TEntity> r) => r.IsSuccess;
}

public record Result<TEntity, TError>(TEntity Value, bool IsSuccess, TError? Error)
{
    // End goal is to use this type to provide a union of two sub types, with the type of TError a struct and class respectively.
    // See: https://spencerfarley.com/2021/03/26/unions-in-csharp/ for explanation of why.
    // Until proper discriminated unions are available.
    public static implicit operator bool(Result<TEntity, TError> r) => r.IsSuccess;
}

public static class ResultExtensions
{
    public static Result<TEntity> And<TEntity>(this Result<TEntity> r, Func<TEntity, bool> predicate, string error = "")
        => r with { IsSuccess = r.IsSuccess && predicate(r.Value), Error = error };

    public static Result<TEntity, TError> And<TEntity, TError>(
        this Result<TEntity, TError> r, Func<TEntity, bool> predicate, TError error)
    {
        if (r.IsSuccess && predicate(r.Value))
        {
            return r;
        }
        return r with { IsSuccess = false, Error = error };
    }

    public static Result<TEntity, TError> And<TEntity, TError>(
        this Result<TEntity, TError> r, Func<TEntity, bool> predicate, Func<TEntity, TError> errorGenerator)
    {
        if (r.IsSuccess && predicate(r.Value))
        {
            return r;
        }
        return r with { IsSuccess = false, Error = errorGenerator(r.Value) };
    }

    // needs a unit test!
    public static Result<TEntity> AndAll<TEntity>(this Result<TEntity> r, params Func<TEntity, bool>[] checks)
    {
        if (checks.Length == 0) { return new(r.Value, true); }
        var firstCheck = checks[0];

        var currentResult = r;
        for (var i = 0; i < checks.Length && currentResult.IsSuccess; i++)
        {
            currentResult = currentResult.And(checks[i]);
        }

        return currentResult;
    }
}

/********* Below this line is another library needs to be moved out ignore for now ************/
public static class Parse<TEntity>
{
    public static ParseSeed<TEntity> This(TEntity value) => new ParseSeed<TEntity>(value);
}
public record ParseSeed<TEntity>(TEntity Value);
public record ParseResult<TEntity>
{
    public TEntity? Value { get; }
    public bool IsParsedSuccessfully { get; }

    private ParseResult(TEntity? value, bool isParsedSuccessfully)
    {
        Value = value;
        IsParsedSuccessfully = isParsedSuccessfully;
    }

    private ParseResult(bool isParsedSuccesffully)
    {
        IsParsedSuccessfully = isParsedSuccesffully;
    }

    public static ParseResult<TEntity> Success(TEntity value) => new ParseResult<TEntity>(value, true);
    public static ParseResult<TEntity> Failure() => new ParseResult<TEntity>(false);

}
public static class ParseSeedExtensions
{
    public static ParseResult<K> As<T, K>(this ParseSeed<T> seed, Func<T, K> parser)
    {
        try
        {
            return ParseResult<K>.Success(parser(seed.Value));
        }
        catch (Exception)
        {
            return ParseResult<K>.Failure();
        }
    }
}
