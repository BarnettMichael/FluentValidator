namespace FluentValidator.Parse;

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
