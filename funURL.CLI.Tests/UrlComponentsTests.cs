using funURL.CLI.Core;

namespace funURL.CLI.Tests;

public class UrlComponentsTests
{
    [Fact]
    public void FromUri_ParsesSimpleUrl()
    {
        var uri = new Uri("https://example.com/path");
        var components = UrlComponents.FromUri(uri);

        Assert.Equal("https", components.Scheme);
        Assert.Equal("example.com", components.Host);
        Assert.Equal("com", components.Tld);
        Assert.Equal("", components.Subdomain);
        Assert.Equal("(default)", components.Port);
        Assert.Equal("/path", components.Path);
        Assert.Equal("", components.Query);
        Assert.Equal("", components.Fragment);
    }

    [Fact]
    public void FromUri_ParsesComplexUrl()
    {
        var uri = new Uri("https://api.dev.example.com:8080/v1/users?sort=name&limit=10#results");
        var components = UrlComponents.FromUri(uri);

        Assert.Equal("https", components.Scheme);
        Assert.Equal("api.dev.example.com", components.Host);
        Assert.Equal("com", components.Tld);
        Assert.Equal("api.dev", components.Subdomain);
        Assert.Equal("8080", components.Port);
        Assert.Equal("/v1/users", components.Path);
        Assert.Equal("sort=name&limit=10", components.Query);
        Assert.Equal("results", components.Fragment);
    }

    [Theory]
    [InlineData("https://sub.example.com", "sub")]
    [InlineData("https://deep.sub.example.com", "deep.sub")]
    [InlineData("https://example.com", "")]
    [InlineData("https://localhost", "")]
    public void FromUri_ExtractsSubdomainCorrectly(string url, string expectedSubdomain)
    {
        var uri = new Uri(url);
        var components = UrlComponents.FromUri(uri);

        Assert.Equal(expectedSubdomain, components.Subdomain);
    }

    [Theory]
    [InlineData("https://example.com", "com")]
    [InlineData("https://example.org", "org")]
    [InlineData("https://sub.example.co.uk", "uk")]
    [InlineData("https://localhost", "")]
    public void FromUri_ExtractsTldCorrectly(string url, string expectedTld)
    {
        var uri = new Uri(url);
        var components = UrlComponents.FromUri(uri);

        Assert.Equal(expectedTld, components.Tld);
    }

    [Theory]
    [InlineData("https://example.com", "(default)")]
    [InlineData("https://example.com:443", "(default)")]
    [InlineData("http://example.com:80", "(default)")]
    [InlineData("https://example.com:8080", "8080")]
    [InlineData("http://example.com:3000", "3000")]
    public void FromUri_HandlesPortCorrectly(string url, string expectedPort)
    {
        var uri = new Uri(url);
        var components = UrlComponents.FromUri(uri);

        Assert.Equal(expectedPort, components.Port);
    }

    [Fact]
    public void FromUri_TrimsQueryAndFragmentMarkers()
    {
        var uri = new Uri("https://example.com/path?query=value#fragment");
        var components = UrlComponents.FromUri(uri);

        Assert.Equal("query=value", components.Query);
        Assert.Equal("fragment", components.Fragment);
        Assert.DoesNotContain("?", components.Query);
        Assert.DoesNotContain("#", components.Fragment);
    }

    [Fact]
    public void FromUri_HandlesEmptyQueryAndFragment()
    {
        var uri = new Uri("https://example.com/path");
        var components = UrlComponents.FromUri(uri);

        Assert.Equal("", components.Query);
        Assert.Equal("", components.Fragment);
    }

    [Fact]
    public void FormatAll_ProducesReadableOutput()
    {
        var uri = new Uri("https://www.example.com/path?query=value#section");
        var components = UrlComponents.FromUri(uri);
        var formatted = components.FormatAll();

        Assert.Contains("Scheme:", formatted);
        Assert.Contains("https", formatted);
        Assert.Contains("Subdomain:", formatted);
        Assert.Contains("www", formatted);
        Assert.Contains("Host:", formatted);
        Assert.Contains("www.example.com", formatted);
        Assert.Contains("TLD:", formatted);
        Assert.Contains("com", formatted);
        Assert.Contains("Path:", formatted);
        Assert.Contains("/path", formatted);
        Assert.Contains("Query:", formatted);
        Assert.Contains("query=value", formatted);
        Assert.Contains("Fragment:", formatted);
        Assert.Contains("section", formatted);
    }

    [Fact]
    public void FromUri_IsImmutable()
    {
        var uri = new Uri("https://example.com/path");
        var components = UrlComponents.FromUri(uri);

        // Verify it's a record struct (immutable)
        var components2 = components with { Scheme = "http" };

        Assert.Equal("https", components.Scheme);
        Assert.Equal("http", components2.Scheme);
    }
}
