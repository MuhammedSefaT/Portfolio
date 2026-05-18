namespace Portfolio.Common.Result;

public class Result<T>
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string>? Errors { get; set; }
    public T? Data { get; init; }

    public static Result<T> Ok(T data)
    {
        return new Result<T>
        {
            Success = true,
            Data = data
        };
    }

    public static Result<T> Fail(string errorMessage)
    {
        return new Result<T>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    public static Result<T> Fail(List<string> errors)
    {
        return new Result<T>
        {
            Success = false,
            Errors = errors
        };
    }
}
