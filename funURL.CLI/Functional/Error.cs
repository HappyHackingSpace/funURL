namespace funURL.CLI.Functional;

internal sealed class Error(string message)
{
    public string Message { get; } = message;

    public static Error New(string message) => new(message);
}
