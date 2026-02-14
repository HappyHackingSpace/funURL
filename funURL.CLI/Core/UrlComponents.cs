namespace funURL.CLI.Core;

/// <summary>
/// Representing parsed URL components.
/// All components are extracted deterministically from the URI.
/// </summary>
/// <param name="Scheme">The URL scheme (e.g., "https", "http")</param>
/// <param name="Subdomain">The subdomain portion (e.g., "www" from "www.example.com")</param>
/// <param name="Host">The full hostname (e.g., "www.example.com")</param>
/// <param name="Tld">The top-level domain (e.g., "com")</param>
/// <param name="Port">The port number or "(default)" if using the default port for the scheme</param>
/// <param name="Path">The absolute path</param>
/// <param name="Query">The query string without the leading '?'</param>
/// <param name="Fragment">The fragment without the leading '#'</param>
internal readonly record struct UrlComponents(
    string Scheme,
    string Subdomain,
    string Host,
    string Tld,
    string Port,
    string Path,
    string Query,
    string Fragment)
{
    /// <summary>
    /// Creates a UrlComponents instance from a URI.
    /// </summary>
    /// <param name="uri">The URI to parse</param>
    /// <returns>A new UrlComponents with all components extracted</returns>
    public static UrlComponents FromUri(Uri uri) => new(
        Scheme: uri.Scheme,
        Subdomain: GetSubdomain(uri),
        Host: uri.Host,
        Tld: GetTld(uri),
        Port: uri.IsDefaultPort ? "(default)" : uri.Port.ToString(),
        Path: uri.AbsolutePath,
        Query: uri.Query.TrimStart('?'),
        Fragment: uri.Fragment.TrimStart('#')
    );

    /// <summary>
    /// Extracts the subdomain from a URI's host.
    /// For example: "api.dev.example.com" returns "api.dev"
    /// </summary>
    /// <param name="uri">The URI to extract from</param>
    /// <returns>The subdomain or empty string if no subdomain exists</returns>
    private static string GetSubdomain(Uri uri)
    {
        var parts = uri.Host.Split('.');
        return parts.Length > 2 ? string.Join('.', parts[..^2]) : string.Empty;
    }

    /// <summary>
    /// Extracts the top-level domain from a URI's host.
    /// For example: "www.example.com" returns "com"
    /// </summary>
    /// <param name="uri">The URI to extract from</param>
    /// <returns>The TLD or empty string if the host has no dots</returns>
    private static string GetTld(Uri uri)
    {
        var parts = uri.Host.Split('.');
        return parts.Length >= 2 ? parts[^1] : string.Empty;
    }

    /// <summary>
    /// Formats all components as a multi-line string suitable for console output.
    /// </summary>
    /// <returns>Formatted string with all URL components</returns>
    public string FormatAll() =>
        $"""
        Scheme:     {Scheme}
        Subdomain:  {Subdomain}
        Host:       {Host}
        TLD:        {Tld}
        Port:       {Port}
        Path:       {Path}
        Query:      {Query}
        Fragment:   {Fragment}
        """;
}
