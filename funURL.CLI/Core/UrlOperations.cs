using System.Net;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace funURL.CLI.Core;

/// <summary>
/// URL operations that don't perform I/O or have side effects.
/// All methods are deterministic and thread-safe.
/// </summary>
internal static class UrlOperations
{
    /// <summary>
    /// Encodes a string for safe use in URLs.
    /// </summary>
    /// <param name="input">The string to encode</param>
    /// <param name="asQuery">If true, uses query component encoding (+ for spaces); otherwise uses URI data string encoding</param>
    /// <returns>The encoded string</returns>
    public static string Encode(string input, bool asQuery) =>
        asQuery ? WebUtility.UrlEncode(input) : Uri.EscapeDataString(input);

    /// <summary>
    /// Decodes a URL-encoded string.
    /// </summary>
    /// <param name="input">The encoded string</param>
    /// <param name="asQuery">If true, uses query component decoding (+ becomes space); otherwise uses URI data string decoding</param>
    /// <returns>The decoded string</returns>
    public static string Decode(string input, bool asQuery) =>
        asQuery ? WebUtility.UrlDecode(input) : Uri.UnescapeDataString(input);

    /// <summary>
    /// Creates a new URI with modified components.
    /// </summary>
    /// <param name="uri">The original URI</param>
    /// <param name="scheme">New scheme (e.g., "https") or null to keep existing</param>
    /// <param name="path">New path or null to keep existing</param>
    /// <param name="query">New query string or null to keep existing</param>
    /// <param name="fragment">New fragment or null to keep existing</param>
    /// <returns>A new URI with the specified modifications</returns>
    public static Uri Modify(Uri uri, string? scheme, string? path, string? query, string? fragment)
    {
        var builder = new UriBuilder(uri);

        if (scheme is not null)
            builder.Scheme = scheme;
        if (path is not null)
            builder.Path = path;
        if (query is not null)
            builder.Query = query;
        if (fragment is not null)
            builder.Fragment = fragment;

        return builder.Uri;
    }

    /// <summary>
    /// Validates and parses a URL string into a URI.
    /// </summary>
    /// <param name="url">The URL string to parse</param>
    /// <returns>A Fin containing the parsed URI or an error</returns>
    public static Fin<Uri> ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Error.New("URL cannot be empty");

        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            ? Fin<Uri>.Succ(uri)
            : Error.New($"Invalid URL: {url}");
    }

    /// <summary>
    /// Validates a non-empty string input.
    /// </summary>
    /// <param name="input">The input string to validate</param>
    /// <param name="parameterName">Name of the parameter for error messages</param>
    /// <returns>A Fin containing the input or an error</returns>
    public static Fin<string> ValidateInput(string input, string parameterName = "Input")
    {
        if (string.IsNullOrWhiteSpace(input))
            return Error.New($"{parameterName} cannot be empty");

        return Fin<string>.Succ(input);
    }
}
