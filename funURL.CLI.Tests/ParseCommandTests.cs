using funURL.CLI.Commands;

namespace funURL.CLI.Tests;

public class ParseCommandTests
{
    [Theory]
    [InlineData("https://sub.example.com", "sub")]
    [InlineData("https://deep.sub.example.com", "deep.sub")]
    [InlineData("https://example.com", "")]
    [InlineData("https://localhost", "")]
    public void GetSubdomain_ExtractsCorrectly(string url, string expected)
    {
        var uri = new Uri(url);
        Assert.Equal(expected, ParseCommand.GetSubdomain(uri));
    }

    [Theory]
    [InlineData("https://example.com", "com")]
    [InlineData("https://example.org", "org")]
    [InlineData("https://sub.example.co.uk", "uk")]
    [InlineData("https://localhost", "")]
    public void GetTld_ExtractsCorrectly(string url, string expected)
    {
        var uri = new Uri(url);
        Assert.Equal(expected, ParseCommand.GetTld(uri));
    }
}
