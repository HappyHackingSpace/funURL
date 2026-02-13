using System.CommandLine;

namespace funURL.CLI.Commands;

public class ModifyCommand : Command
{
    private readonly Argument<string> _urlArgument = new("url") { Description = "URL to modify" };

    private readonly Option<string?> _protocolOption = new("--protocol", "-c") { Description = "Change protocol/scheme" };

    private readonly Option<string?> _pathOption = new("--path", "-p") { Description = "Update path" };

    private readonly Option<string?> _queryOption = new("--query", "-q") { Description = "Change query string" };

    private readonly Option<string?> _fragmentOption = new("--fragment", "-f") { Description = "Update fragment" };

    private ModifyCommand()
        : base("modify", "Modify components of a URL")
    {
        Arguments.Add(_urlArgument);
        Options.Add(_protocolOption);
        Options.Add(_pathOption);
        Options.Add(_queryOption);
        Options.Add(_fragmentOption);

        SetAction(
            async (parseResult, cancellationToken) =>
            {
                var url = parseResult.GetValue(_urlArgument)!;

                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    await Console.Error.WriteLineAsync($"Invalid URL: {url}".AsMemory(), cancellationToken);
                    return;
                }

                var protocol = parseResult.GetValue(_protocolOption);
                var path = parseResult.GetValue(_pathOption);
                var query = parseResult.GetValue(_queryOption);
                var fragment = parseResult.GetValue(_fragmentOption);

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
