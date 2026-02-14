using System.CommandLine;
using funURL.CLI.Core;
using LanguageExt;
using static LanguageExt.Prelude;

namespace funURL.CLI.Commands;

/// <summary>
/// Command to modify URL components.
/// </summary>
public class ModifyCommand : Command
{
    private readonly Argument<string> urlArgument = new("url") { Description = "URL to modify" };
    private readonly System.CommandLine.Option<string?> protocolOption = new("--protocol", "-c")
    {
        Description = "Change protocol/scheme"
    };
    private readonly System.CommandLine.Option<string?> pathOption = new("--path", "-p")
    {
        Description = "Update path"
    };
    private readonly System.CommandLine.Option<string?> queryOption = new("--query", "-q")
    {
        Description = "Change query string"
    };
    private readonly System.CommandLine.Option<string?> fragmentOption = new("--fragment", "-f")
    {
        Description = "Update fragment"
    };

    private ModifyCommand()
        : base("modify", "Modify components of a URL")
    {
        Arguments.Add(urlArgument);
        Options.Add(protocolOption);
        Options.Add(pathOption);
        Options.Add(queryOption);
        Options.Add(fragmentOption);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var url = parseResult.GetValue(urlArgument)!;
            var protocol = parseResult.GetValue(protocolOption);
            var path = parseResult.GetValue(pathOption);
            var query = parseResult.GetValue(queryOption);
            var fragment = parseResult.GetValue(fragmentOption);

            var result = UrlOperations.ValidateUrl(url)
                .Map(uri => UrlOperations.Modify(uri, protocol, path, query, fragment))
                .Map(modified => modified.ToString());

            await result.Match(
                Succ: async value => await Console.Out.WriteLineAsync(value.AsMemory(), cancellationToken),
                Fail: async error => await Console.Error.WriteLineAsync(error.Message.AsMemory(), cancellationToken)
            );
        });
    }

    public static ModifyCommand Create() => new();
}
