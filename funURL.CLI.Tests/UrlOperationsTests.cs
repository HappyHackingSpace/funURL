using funURL.CLI.Core;

namespace funURL.CLI.Tests;

public class UrlOperationsTests
{
    [Theory]
    [InlineData("hello world", false, "hello%20world")]
    [InlineData("hello world", true, "hello+world")]
    [InlineData("hello@example.com", false, "hello%40example.com")]
    [InlineData("foo&bar=baz", true, "foo%26bar%3Dbaz")]
    [InlineData("", false, "")]
    [InlineData("", true, "")]
    public void Encode_EncodesCorrectly(string input, bool asQuery, string expected)
    {
        var result = UrlOperations.Encode(input, asQuery);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("hello%20world", false, "hello world")]
    [InlineData("hello+world", true, "hello world")]
    [InlineData("hello%40example.com", false, "hello@example.com")]
    [InlineData("foo%26bar%3dbaz", true, "foo&bar=baz")]
    [InlineData("", false, "")]
    [InlineData("", true, "")]
    public void Decode_DecodesCorrectly(string input, bool asQuery, string expected)
    {
        var result = UrlOperations.Decode(input, asQuery);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Encode_Decode_RoundTrip_DataString()
    {
        var original = "hello@world! #test";
        var encoded = UrlOperations.Encode(original, asQuery: false);
        var decoded = UrlOperations.Decode(encoded, asQuery: false);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Encode_Decode_RoundTrip_Query()
    {
        var original = "hello world & test=value";
        var encoded = UrlOperations.Encode(original, asQuery: true);
        var decoded = UrlOperations.Decode(encoded, asQuery: true);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Modify_ChangesScheme()
    {
        var uri = new Uri("http://example.com/path");
        var modified = UrlOperations.Modify(uri, scheme: "https", path: null, query: null, fragment: null);

        Assert.Equal("https", modified.Scheme);
        Assert.Equal("/path", modified.AbsolutePath);
    }

    [Fact]
    public void Modify_ChangesPath()
    {
        var uri = new Uri("https://example.com/old");
        var modified = UrlOperations.Modify(uri, scheme: null, path: "/new", query: null, fragment: null);

        Assert.Equal("https", modified.Scheme);
        Assert.Equal("/new", modified.AbsolutePath);
    }

    [Fact]
    public void Modify_ChangesQuery()
    {
        var uri = new Uri("https://example.com/path?old=value");
        var modified = UrlOperations.Modify(uri, scheme: null, path: null, query: "new=value", fragment: null);

        Assert.Equal("?new=value", modified.Query);
    }

    [Fact]
    public void Modify_ChangesFragment()
    {
        var uri = new Uri("https://example.com/path#old");
        var modified = UrlOperations.Modify(uri, scheme: null, path: null, query: null, fragment: "new");

        Assert.Equal("#new", modified.Fragment);
    }

    [Fact]
    public void Modify_ChangesMultipleComponents()
    {
        var uri = new Uri("http://example.com/old?foo=bar#baz");
        var modified = UrlOperations.Modify(uri, scheme: "https", path: "/new", query: "test=value", fragment: "section");

        Assert.Equal("https", modified.Scheme);
        Assert.Equal("/new", modified.AbsolutePath);
        Assert.Equal("?test=value", modified.Query);
        Assert.Equal("#section", modified.Fragment);
    }

    [Fact]
    public void Modify_PreservesUnchangedComponents()
    {
        var uri = new Uri("https://example.com:8080/path?query=value#fragment");
        var modified = UrlOperations.Modify(uri, scheme: null, path: null, query: null, fragment: null);

        Assert.Equal(uri.ToString(), modified.ToString());
    }

    [Fact]
    public void ValidateUrl_SucceedsForValidUrls()
    {
        var result = UrlOperations.ValidateUrl("https://example.com");

        Assert.True(result.IsSuccess);
        Assert.Equal("https", result.Value.Scheme);
        Assert.Equal("example.com", result.Value.Host);
    }

    [Fact]
    public void ValidateUrl_SucceedsForComplexUrl()
    {
        var result = UrlOperations.ValidateUrl("http://sub.example.com/path?query=value#fragment");

        Assert.True(result.IsSuccess);
        Assert.Equal("http", result.Value.Scheme);
        Assert.Equal("sub.example.com", result.Value.Host);
        Assert.Equal("/path", result.Value.AbsolutePath);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-url")]
    [InlineData("http://")]
    [InlineData("://example.com")]
    public void ValidateUrl_FailsForInvalidUrls(string url)
    {
        var result = UrlOperations.ValidateUrl(url);

        Assert.False(result.IsSuccess);
        Assert.Contains("URL", result.Error);
    }

    [Theory]
    [InlineData("hello", "Input")]
    [InlineData("test value", "Input")]
    public void ValidateInput_SucceedsForNonEmptyStrings(string input, string paramName)
    {
        var result = UrlOperations.ValidateInput(input, paramName);

        Assert.True(result.IsSuccess);
        Assert.Equal(input, result.Value);
    }

    [Theory]
    [InlineData("", "Input")]
    [InlineData("   ", "Input")]
    [InlineData("", "CustomParam")]
    public void ValidateInput_FailsForEmptyStrings(string input, string paramName)
    {
        var result = UrlOperations.ValidateInput(input, paramName);

        Assert.False(result.IsSuccess);
        Assert.Contains(paramName, result.Error);
        Assert.Contains("cannot be empty", result.Error);
    }
}
