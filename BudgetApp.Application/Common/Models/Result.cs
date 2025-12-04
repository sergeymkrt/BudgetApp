namespace BudgetApp.Application.Common.Models;

/// <summary>
/// Generic result wrapper for operations that can fail.
/// </summary>
public class Result
{
    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public bool Succeeded { get; }
    public string[] Errors { get; }

    public static Result Success() => new(true, []);
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    public static Result Failure(string error) => new(false, [error]);

    /// <summary>
    /// Pattern match on the result.
    /// </summary>
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string[], TResult> onFailure)
        => Succeeded ? onSuccess() : onFailure(Errors);
}

/// <summary>
/// Generic result wrapper with data payload.
/// </summary>
public class Result<T> : Result
{
    private Result(bool succeeded, T? data, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Data = data;
    }

    public T? Data { get; }

    public static Result<T> Success(T data) => new(true, data, []);
    public static new Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
    public static new Result<T> Failure(string error) => new(false, default, [error]);

    /// <summary>
    /// Pattern match on the result with data.
    /// </summary>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string[], TResult> onFailure)
        => Succeeded && Data is not null ? onSuccess(Data) : onFailure(Errors);
}

