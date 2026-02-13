using System.CommandLine;

namespace funURL.CLI.Commands;

public class ModifyCommand : Command
{
    private readonly Argument<string> urlArgument = new("url") { Description = "URL to modify" };

    private readonly Option<string?> protocolOption = new("--protocol", "-c") { Description = "Change protocol/scheme" };

    private readonly Option<string?> pathOption = new("--path", "-p") { Description = "Update path" };

    private readonly Option<string?> queryOption = new("--query", "-q") { Description = "Change query string" };

    private readonly Option<string?> fragmentOption = new("--fragment", "-f") { Description = "Update fragment" };

    private ModifyCommand()
        : base("modify", "Modify components of a URL")
    {
        Arguments.Add(urlArgument);
        Options.Add(protocolOption);
        Options.Add(pathOption);
        Options.Add(queryOption);
        Options.Add(fragmentOption);

        SetAction(
            async (parseResult, cancellationToken) =>
            {
                var url = parseResult.GetValue(urlArgument)!;

                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    await Console.Error.WriteLineAsync($"Invalid URL: {url}".AsMemory(), cancellationToken);
                    return;
                }

                var protocol = parseResult.GetValue(protocolOption);
                var path = parseResult.GetValue(pathOption);
                var query = parseResult.GetValue(queryOption);
                var fragment = parseResult.GetValue(fragmentOption);

                var builder = new UriBuilder(uri);

                if (protocol is not null)
                    builder.Scheme = protocol;
                if (path is not null)
                    builder.Path = path;
                if (query is not null)
                    builder.Query = query;
                if (fragment is not null)
                    builder.Fragment = fragment;

                await Console.Out.WriteLineAsync(builder.Uri.ToString().AsMemory(), cancellationToken);
            }
        );
    }

    public static ModifyCommand Create() => new();
}
