using System.CommandLine;
using System.Net;

namespace funURL.CLI.Commands;

public class DecodeCommand : Command
{
    private readonly Argument<string> _inputArgument = new("input") { Description = "String to decode" };

    private readonly Option<bool> _queryOption = new("--query", "-c")
    {
        Description = "Decode as query component (handles + as spaces)",
    };

    private DecodeCommand()
        : base("decode", "URL-decode a string")
    {
        Arguments.Add(_inputArgument);
        Options.Add(_queryOption);

        SetAction(
            async (parseResult, cancellationToken) =>
            {
                var input = parseResult.GetValue(_inputArgument)!;
                var asQuery = parseResult.GetValue(_queryOption);

                var decoded = asQuery ? WebUtility.UrlDecode(input) : Uri.UnescapeDataString(input);

                await Console.Out.WriteLineAsync(decoded.AsMemory(), cancellationToken);
            }
        );
    }

    public static DecodeCommand Create() => new();
}
