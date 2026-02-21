using System.CommandLine;
using System.Text.RegularExpressions;
using funURL.CLI.Core;
using funURL.CLI.Functional;

namespace funURL.CLI.Commands;

/// <summary>
/// Command to remove duplicate URLs based on their structure.
/// URLs are considered duplicates if they have the same scheme, host, path structure, and query parameter names.
/// </summary>
public partial class DedupeCommand : Command
{
    private readonly Argument<string[]> urlsArgument = new("urls") { Description = "URLs to deduplicate", Arity = ArgumentArity.ZeroOrMore };

    private DedupeCommand()
        : base("dedupe", "Remove duplicate URLs based on structure")
    {
        Arguments.Add(urlsArgument);

        SetAction(
            async (parseResult, cancellationToken) =>
            {
                var urls = parseResult.GetValue(urlsArgument) ?? [];

                if (urls.Length == 0)
                {
                    urls = await ReadFromStdinAsync(cancellationToken);
                }

                var uniqueUrls = urls.Select(url => (Original: url.Trim(), Parsed: UrlOperations.ValidateUrl(url.Trim())))
                    .Where(x => x.Parsed.IsSucc)
                    .Select(x => (x.Original, Key: GetStructureKey(x.Parsed.ThrowIfFail())))
                    .DistinctBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(x => x.Original);

                foreach (var url in uniqueUrls)
                {
                    await Console.Out.WriteLineAsync(url.AsMemory(), cancellationToken);
                }
            }
        );
    }

    public static DedupeCommand Create() => new();

    internal static string GetStructureKey(Uri uri)
    {
        var normalizedPath = NormalizePath(uri.AbsolutePath);
        var paramNames = GetSortedParamNames(uri.Query);
        return $"{uri.Scheme}://{uri.Host}{normalizedPath}?{paramNames}";
    }

    internal static string NormalizePath(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];

            // Replace numeric IDs
            if (NumericRegex().IsMatch(segment))
            {
                segments[i] = "<id>";
                continue;
            }

            // Replace UUIDs
            if (UuidRegex().IsMatch(segment))
            {
                segments[i] = "<id>";
                continue;
            }

            // Normalize filenames but keep extension
            var dotIndex = segment.LastIndexOf('.');
            if (dotIndex > 0)
            {
                var ext = segment[dotIndex..];
                segments[i] = $"<file>{ext}";
            }
        }

        return "/" + string.Join('/', segments);
    }

    internal static string GetSortedParamNames(string query)
    {
        if (string.IsNullOrEmpty(query))
            return string.Empty;

        var keys = query
            .TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split('=', 2)[0])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase);

        return string.Join('&', keys);
    }

    private static async Task<string[]> ReadFromStdinAsync(CancellationToken cancellationToken)
    {
        var lines = new List<string>();

        while (await Console.In.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line);
        }

        return lines.ToArray();
    }

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex NumericRegex();

    [GeneratedRegex(@"^[0-9a-f]{8}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{12}$", RegexOptions.IgnoreCase)]
    private static partial Regex UuidRegex();
}
