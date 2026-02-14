using System.CommandLine;
using funURL.CLI.Core;

namespace funURL.CLI.Commands;

/// <summary>
/// Command to URL-encode strings.
/// </summary>
public class EncodeCommand : Command
{
    private readonly Argument<string> inputArgument = new("input") { Description = "String to encode" };
    private readonly Option<bool> queryOption = new("--query", "-c")
    {
        Description = "Encode as query component (uses + for spaces)"
    };

    private EncodeCommand()
        : base("encode", "URL-encode a string")
    {
        Arguments.Add(inputArgument);
        Options.Add(queryOption);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var input = parseResult.GetValue(inputArgument)!;
            var asQuery = parseResult.GetValue(queryOption);

            var result = UrlOperations.ValidateInput(input, "Input")
                .Map(validInput => UrlOperations.Encode(validInput, asQuery));

            if (result.IsSuccess)
                await Console.Out.WriteLineAsync(result.Value.AsMemory(), cancellationToken);
            else
                await Console.Error.WriteLineAsync(result.Error.AsMemory(), cancellationToken);
        });
    }

    public static EncodeCommand Create() => new();
}
