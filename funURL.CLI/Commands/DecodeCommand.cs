using System.CommandLine;
using funURL.CLI.Core;

namespace funURL.CLI.Commands;

/// <summary>
/// Command to URL-decode strings.
/// </summary>
public class DecodeCommand : Command
{
    private readonly Argument<string> inputArgument = new("input") { Description = "String to decode" };
    private readonly Option<bool> queryOption = new("--query", "-c")
    {
        Description = "Decode as query component (handles + as spaces)"
    };

    private DecodeCommand()
        : base("decode", "URL-decode a string")
    {
        Arguments.Add(inputArgument);
        Options.Add(queryOption);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var input = parseResult.GetValue(inputArgument)!;
            var asQuery = parseResult.GetValue(queryOption);

            var result = UrlOperations.ValidateInput(input, "Input")
                .Map(validInput => UrlOperations.Decode(validInput, asQuery));

            if (result.IsSuccess)
                await Console.Out.WriteLineAsync(result.Value.AsMemory(), cancellationToken);
            else
                await Console.Error.WriteLineAsync(result.Error.AsMemory(), cancellationToken);
        });
    }

    public static DecodeCommand Create() => new();
}
