namespace FluentValidator.Validate;

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
