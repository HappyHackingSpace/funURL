namespace funURL.CLI.Functional;

internal abstract class Fin<T>
{
    public abstract bool IsSucc { get; }
    public bool IsFail => !IsSucc;

    public static Fin<T> Succ(T value) => new SuccCase(value);

    public static implicit operator Fin<T>(Error error) => new FailCase(error);

    public abstract Fin<TResult> Map<TResult>(Func<T, TResult> mapper);

    public abstract Task Match(Func<T, Task> Succ, Func<Error, Task> Fail);

    public abstract T ThrowIfFail();

    private sealed class SuccCase(T value) : Fin<T>
    {
        public override bool IsSucc => true;

        public override Fin<TResult> Map<TResult>(Func<T, TResult> mapper) => Fin<TResult>.Succ(mapper(value));

        public override Task Match(Func<T, Task> Succ, Func<Error, Task> Fail) => Succ(value);

        public override T ThrowIfFail() => value;
    }

    private sealed class FailCase(Error error) : Fin<T>
    {
        public override bool IsSucc => false;

        public override Fin<TResult> Map<TResult>(Func<T, TResult> mapper) => error;

        public override Task Match(Func<T, Task> Succ, Func<Error, Task> Fail) => Fail(error);

        public override T ThrowIfFail() => throw new InvalidOperationException(error.Message);
    }
}
