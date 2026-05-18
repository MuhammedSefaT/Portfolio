namespace Portfolio.Common.Result;

public class Result
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string>? Errors { get; set; }

    public static Result Ok()
    {
        return new Result
        {
            Success = true
        };
    }

    public static Result Fail(string errorMessage)
    {
        return new Result
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    public static Result Fail(List<string> errors)
    {
        return new Result
        {
            Success = false,
            Errors = errors
        };
    }
}
