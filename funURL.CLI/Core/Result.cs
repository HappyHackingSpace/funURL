namespace funURL.CLI.Core;

/// <summary>
/// Represents a computation that can either succeed with a value or fail with an error message.
/// Provides explicit error handling without exceptions.
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
internal readonly record struct Result<T>
{
    private readonly T? _value;
    private readonly string? _error;

    /// <summary>
    /// Gets a value indicating whether the result represents a successful operation.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the success value. Throws if the result is a failure.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing Value on a failed result</exception>
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException(_error);

    /// <summary>
    /// Gets the error message. Throws if the result is a success.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing Error on a successful result</exception>
    public string Error => !IsSuccess ? _error! : throw new InvalidOperationException("Cannot access Error on a successful result");

    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        _error = null;
    }

    private Result(string error)
    {
        IsSuccess = false;
        _value = default;
        _error = error;
    }

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    public static Result<T> Failure(string error) => new(error);

    /// <summary>
    /// Transforms the success value using the specified mapper function.
    /// If this result is a failure, returns a new failure with the same error.
    /// </summary>
    /// <typeparam name="TOut">The type of the transformed value</typeparam>
    /// <param name="mapper">Function to transform the success value</param>
    /// <returns>A new result with the transformed value or the original error</returns>
    public Result<TOut> Map<TOut>(Func<T, TOut> mapper) =>
        IsSuccess ? Result<TOut>.Success(mapper(_value!)) : Result<TOut>.Failure(_error!);

    /// <summary>
    /// Chains another result-producing operation.
    /// If this result is a failure, returns a new failure with the same error.
    /// </summary>
    /// <typeparam name="TOut">The type of the output result</typeparam>
    /// <param name="binder">Function that produces a new result from the success value</param>
    /// <returns>The result of the binder function or the original error</returns>
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder) =>
        IsSuccess ? binder(_value!) : Result<TOut>.Failure(_error!);
}
