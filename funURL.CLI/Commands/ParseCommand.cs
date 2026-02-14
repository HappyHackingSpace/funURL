using System.CommandLine;
using funURL.CLI.Core;
using LanguageExt;
using static LanguageExt.Prelude;

namespace funURL.CLI.Commands;

/// <summary>
/// Command to parse and extract components from URLs.
/// </summary>
public class ParseCommand : Command
{
    private readonly Argument<string> urlArgument = new("url") { Description = "URL to parse" };
    private readonly System.CommandLine.Option<bool> protocolOption = new("--protocol", "-c")
    {
        Description = "Extract protocol/scheme"
    };
    private readonly System.CommandLine.Option<bool> subdomainOption = new("--subdomain", "-s")
    {
        Description = "Extract subdomain"
    };
    private readonly System.CommandLine.Option<bool> tldOption = new("--tld", "-t")
    {
        Description = "Extract top-level domain"
    };
    private readonly System.CommandLine.Option<bool> hostnameOption = new("--hostname", "-n")
    {
        Description = "Extract hostname"
    };
    private readonly System.CommandLine.Option<bool> pathOption = new("--path", "-p")
    {
        Description = "Extract path"
    };
    private readonly System.CommandLine.Option<bool> queryOption = new("--query", "-q")
    {
        Description = "Extract query parameters"
    };
    private readonly System.CommandLine.Option<bool> fragmentOption = new("--fragment", "-f")
    {
        Description = "Extract fragment"
    };

    private ParseCommand()
        : base("parse", "Parse and extract components from a URL")
    {
        Arguments.Add(urlArgument);
        Options.Add(protocolOption);
        Options.Add(subdomainOption);
        Options.Add(tldOption);
        Options.Add(hostnameOption);
        Options.Add(pathOption);
        Options.Add(queryOption);
        Options.Add(fragmentOption);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var url = parseResult.GetValue(urlArgument)!;

            var result = UrlOperations.ValidateUrl(url).Map(UrlComponents.FromUri);

            await result.Match(
                Fail: async error =>
                {
                    await Console.Error.WriteLineAsync(error.Message.AsMemory(), cancellationToken);
                },
                Succ: async components =>
                {
                    var selectedOptions = new[]
                    {
                        (parseResult.GetValue(protocolOption), components.Scheme),
                        (parseResult.GetValue(subdomainOption), components.Subdomain),
                        (parseResult.GetValue(tldOption), components.Tld),
                        (parseResult.GetValue(hostnameOption), components.Host),
                        (parseResult.GetValue(pathOption), components.Path),
                        (parseResult.GetValue(queryOption), components.Query),
                        (parseResult.GetValue(fragmentOption), components.Fragment)
                    };

                    var anyFlagSet = selectedOptions.Any(opt => opt.Item1);

                    if (!anyFlagSet)
                    {
                        await Console.Out.WriteLineAsync(components.FormatAll().AsMemory(), cancellationToken);
                        return;
                    }

                    foreach (var (isSelected, value) in selectedOptions.Where(opt => opt.Item1))
                    {
                        await Console.Out.WriteLineAsync(value.AsMemory(), cancellationToken);
                    }
                }
            );
        });
    }

    public static ParseCommand Create() => new();

    // Kept for backward compatibility with existing tests
    internal static string GetSubdomain(Uri uri) => UrlComponents.FromUri(uri).Subdomain;
    internal static string GetTld(Uri uri) => UrlComponents.FromUri(uri).Tld;
}
