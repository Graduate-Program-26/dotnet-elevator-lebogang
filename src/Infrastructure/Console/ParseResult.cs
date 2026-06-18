public abstract record ParseResult<T>
{
    public record Success(T Value) : ParseResult<T>;
    public record Failure(string ErrorMessage);

    private ParseResult() {}
}