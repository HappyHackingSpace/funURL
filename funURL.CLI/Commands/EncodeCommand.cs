using System.CommandLine;
using System.Net;

namespace funURL.CLI.Commands;

public class EncodeCommand : Command
{
    private readonly Argument<string> inputArgument = new("input") { Description = "String to encode" };

    private readonly Option<bool> queryOption = new("--query", "-c") { Description = "Encode as query component (uses + for spaces)" };

    private EncodeCommand()
        : base("encode", "URL-encode a string")
    {
        Arguments.Add(inputArgument);
        Options.Add(queryOption);

        SetAction(
            async (parseResult, cancellationToken) =>
            {
                var input = parseResult.GetValue(inputArgument)!;
                var asQuery = parseResult.GetValue(queryOption);

                var encoded = asQuery ? WebUtility.UrlEncode(input) : Uri.EscapeDataString(input);

                await Console.Out.WriteLineAsync(encoded.AsMemory(), cancellationToken);
            }
        );
    }

    public static EncodeCommand Create() => new();
}
