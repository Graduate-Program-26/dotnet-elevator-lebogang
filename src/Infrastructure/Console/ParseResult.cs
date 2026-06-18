public abstract record ParseResult<T>
{
    public record Success(T Value) : ParseResult<T>;
    public record Failure(string ErrorMessage) : ParseResult<T>;

    private ParseResult() {}
}