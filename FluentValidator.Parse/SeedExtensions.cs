namespace FluentValidator.Parse;

public static class SeedExtensions
{
    public static Result<K> As<T, K>(this Seed<T> seed, Func<T, K> parser)
    {
        try
        {
            return Result<K>.Success(parser(seed.Value));
        }
        catch (Exception)
        {
            return Result<K>.Failure();
        }
    }
}
