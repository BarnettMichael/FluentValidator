namespace FluentValidator.Parse;

public record Result<TEntity>
{
    public TEntity? Value { get; }
    public bool IsParsedSuccessfully { get; }

    private Result(TEntity? value, bool isParsedSuccessfully)
    {
        Value = value;
        IsParsedSuccessfully = isParsedSuccessfully;
    }

    private Result(bool isParsedSuccesffully)
    {
        IsParsedSuccessfully = isParsedSuccesffully;
    }

    public static Result<TEntity> Success(TEntity value) => new Result<TEntity>(value, true);
    public static Result<TEntity> Failure() => new Result<TEntity>(false);

}
