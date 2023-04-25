namespace FluentValidator.Validate;

/// <summary>
/// Stores the final result of a validation pipeline. 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <param name="Value"></param>
/// <param name="IsSuccess"></param>
/// <param name="Error"></param>
public record Result<TEntity>(TEntity Value, bool IsSuccess, string Error = "")
{
    public static implicit operator bool(Result<TEntity> r) => r.IsSuccess;

    //public static implicit operator Result<TEntity, TError>(Result<TEntity> r) => new(r.Value, r.IsSuccess, default);
}
public record Result<TEntity, TError>(TEntity Value, bool IsSuccess, TError? Error)
{
    //public static implicit operator bool(Result<TEntity, TError> r) => r.IsSuccess;

    //public static implicit operator Result<TEntity>(Result<TEntity, TError> r) => new(r.Value, r.IsSuccess, r.Error?.ToString() ?? string.Empty);
}