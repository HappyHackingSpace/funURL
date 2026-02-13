using System.CommandLine;

namespace funURL.CLI.Commands;

public class ParseCommand : Command
{
    private readonly Argument<string> _urlArgument = new("url") { Description = "URL to parse" };

    private readonly Option<bool> _protocolOption = new("--protocol", "-c") { Description = "Extract protocol/scheme" };

    private readonly Option<bool> _subdomainOption = new("--subdomain", "-s") { Description = "Extract subdomain" };

    private readonly Option<bool> _tldOption = new("--tld", "-t") { Description = "Extract top-level domain" };

    private readonly Option<bool> _hostnameOption = new("--hostname", "-n") { Description = "Extract hostname" };

    private readonly Option<bool> _pathOption = new("--path", "-p") { Description = "Extract path" };

    private readonly Option<bool> _queryOption = new("--query", "-q") { Description = "Extract query parameters" };

    private readonly Option<bool> _fragmentOption = new("--fragment", "-f") { Description = "Extract fragment" };

    private ParseCommand()
        : base("parse", "Parse and extract components from a URL")
    {
        Arguments.Add(_urlArgument);
        Options.Add(_protocolOption);
        Options.Add(_subdomainOption);
        Options.Add(_tldOption);
        Options.Add(_hostnameOption);
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
                var subdomain = parseResult.GetValue(_subdomainOption);
                var tld = parseResult.GetValue(_tldOption);
                var hostname = parseResult.GetValue(_hostnameOption);
                var path = parseResult.GetValue(_pathOption);
                var query = parseResult.GetValue(_queryOption);
                var fragment = parseResult.GetValue(_fragmentOption);

                var anyFlag = protocol || subdomain || tld || hostname || path || query || fragment;

                if (!anyFlag)
                {
                    await PrintAllAsync(uri, cancellationToken);
                    return;
                }

                if (protocol)
                    await Console.Out.WriteLineAsync(uri.Scheme.AsMemory(), cancellationToken);
                if (subdomain)
                    await Console.Out.WriteLineAsync(GetSubdomain(uri).AsMemory(), cancellationToken);
                if (tld)
                    await Console.Out.WriteLineAsync(GetTld(uri).AsMemory(), cancellationToken);
                if (hostname)
                    await Console.Out.WriteLineAsync(uri.Host.AsMemory(), cancellationToken);
                if (path)
                    await Console.Out.WriteLineAsync(uri.AbsolutePath.AsMemory(), cancellationToken);
                if (query)
                    await Console.Out.WriteLineAsync(uri.Query.TrimStart('?').AsMemory(), cancellationToken);
                if (fragment)
                    await Console.Out.WriteLineAsync(uri.Fragment.TrimStart('#').AsMemory(), cancellationToken);
            }
        );
    }

    public static ParseCommand Create() => new();

    private static async Task PrintAllAsync(Uri uri, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync($"{"Scheme:", -10} {uri.Scheme}".AsMemory(), cancellationToken);
        await Console.Out.WriteLineAsync($"{"Subdomain:", -10} {GetSubdomain(uri)}".AsMemory(), cancellationToken);
        await Console.Out.WriteLineAsync($"{"Host:", -11}{uri.Host}".AsMemory(), cancellationToken);
        await Console.Out.WriteLineAsync($"{"TLD:", -10} {GetTld(uri)}".AsMemory(), cancellationToken);
        await Console.Out.WriteLineAsync(
            $"{"Port:", -10} {(uri.IsDefaultPort ? "(default)" : uri.Port)}".AsMemory(),
            cancellationToken
        );
        await Console.Out.WriteLineAsync($"{"Path:", -10} {uri.AbsolutePath}".AsMemory(), cancellationToken);
        await Console.Out.WriteLineAsync($"{"Query:", -10} {uri.Query.TrimStart('?')}".AsMemory(), cancellationToken);
        await Console.Out.WriteLineAsync($"{"Fragment:", -1} {uri.Fragment.TrimStart('#')}".AsMemory(), cancellationToken);
    }

    internal static string GetSubdomain(Uri uri)
    {
        var parts = uri.Host.Split('.');
        return parts.Length > 2 ? string.Join('.', parts[..^2]) : string.Empty;
    }

    internal static string GetTld(Uri uri)
    {
        var parts = uri.Host.Split('.');
        return parts.Length >= 2 ? parts[^1] : string.Empty;
    }
}
