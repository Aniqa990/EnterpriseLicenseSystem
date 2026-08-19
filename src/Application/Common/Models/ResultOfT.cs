namespace EnterpriseLicenseSystem.Application.Common.Models;

public class Result<T>
{
    internal Result(bool succeeded, T? value, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors.ToArray();
    }

    public bool Succeeded { get; init; }

    public T? Value { get; init; }

    public string[] Errors { get; init; }

    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());

    public static Result<T> Failure(string error) => new(false, default, new[] { error });

    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
}
