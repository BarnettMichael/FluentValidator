namespace FluentValidator.Parse;

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
