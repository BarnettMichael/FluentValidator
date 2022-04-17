namespace FluentValidator.Validate;

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
