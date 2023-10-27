using System.Text.Json.Serialization;

namespace Simbir.Core.Results;

public sealed class Result<T>
{
    public T Value { get; }
    public bool Succeeded { get; }
    public string[] Errors { get; }

    [JsonConstructor]
    private Result(T value, bool succeeded, string[] errors = null!)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value)
        => new(value, true);

    public static Result<T> Failed(params string[] errors)
        => new(default, false, errors);

    public Result ToNonGenericResult()
        => Succeeded ? Result.Success() : Result.Failed(Errors);
}

public sealed class Result
{
    public bool Succeeded { get; }
    public string[] Errors { get; }

    [JsonConstructor]
    private Result(bool succeeded, string[] errors = null!)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public static Result Success()
        => new(true);

    public static Result Failed(params string[] errors)
        => new(false, errors);
}