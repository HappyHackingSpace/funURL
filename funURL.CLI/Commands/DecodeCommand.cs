using System.CommandLine;
using System.Net;

namespace funURL.CLI.Commands;

public class DecodeCommand : Command
{
    private readonly Argument<string> inputArgument = new("input") { Description = "String to decode" };

    private readonly Option<bool> queryOption = new("--query", "-c") { Description = "Decode as query component (handles + as spaces)" };

    private DecodeCommand()
        : base("decode", "URL-decode a string")
    {
        Arguments.Add(inputArgument);
        Options.Add(queryOption);

        SetAction(
            async (parseResult, cancellationToken) =>
            {
                var input = parseResult.GetValue(inputArgument)!;
                var asQuery = parseResult.GetValue(queryOption);

                var decoded = asQuery ? WebUtility.UrlDecode(input) : Uri.UnescapeDataString(input);

                await Console.Out.WriteLineAsync(decoded.AsMemory(), cancellationToken);
            }
        );
    }

    public static DecodeCommand Create() => new();
}
