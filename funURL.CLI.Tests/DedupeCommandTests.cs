using funURL.CLI.Commands;

namespace funURL.CLI.Tests;

public class DedupeCommandTests
{
    [Theory]
    [InlineData("/users/123", "/users/<id>")]
    [InlineData("/users/0", "/users/<id>")]
    [InlineData("/users/999999", "/users/<id>")]
    [InlineData("/users/abc", "/users/abc")]
    [InlineData("/", "/")]
    public void NormalizePath_NumericIds(string input, string expected)
    {
        Assert.Equal(expected, DedupeCommand.NormalizePath(input));
    }

    [Theory]
    [InlineData("/users/550e8400-e29b-41d4-a716-446655440000", "/users/<id>")]
    [InlineData("/users/550e8400e29b41d4a716446655440000", "/users/<id>")]
    [InlineData("/users/not-a-uuid", "/users/not-a-uuid")]
    public void NormalizePath_Uuids(string input, string expected)
    {
        Assert.Equal(expected, DedupeCommand.NormalizePath(input));
    }

    [Theory]
    [InlineData("/assets/style.css", "/assets/<file>.css")]
    [InlineData("/images/photo.jpg", "/images/<file>.jpg")]
    [InlineData("/docs/readme.md", "/docs/<file>.md")]
    [InlineData("/plain", "/plain")]
    public void NormalizePath_Filenames(string input, string expected)
    {
        Assert.Equal(expected, DedupeCommand.NormalizePath(input));
    }

    [Fact]
    public void NormalizePath_MixedSegments()
    {
        var result = DedupeCommand.NormalizePath("/api/users/42/posts/550e8400-e29b-41d4-a716-446655440000/image.png");
        Assert.Equal("/api/users/<id>/posts/<id>/<file>.png", result);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("?sort=name", "sort")]
    [InlineData("?sort=name&filter=active", "filter&sort")]
    [InlineData("?b=2&a=1&c=3", "a&b&c")]
    public void GetSortedParamNames_SortsAndExtractsKeys(string query, string expected)
    {
        Assert.Equal(expected, DedupeCommand.GetSortedParamNames(query));
    }

    [Fact]
    public void GetSortedParamNames_RemovesDuplicates()
    {
        var result = DedupeCommand.GetSortedParamNames("?sort=name&filter=active&sort=date");
        Assert.Equal("filter&sort", result);
    }

    [Fact]
    public void GetStructureKey_SameStructureDifferentValues()
    {
        var uri1 = new Uri("https://example.com/users/1?sort=name");
        var uri2 = new Uri("https://example.com/users/2?sort=date");

        Assert.Equal(DedupeCommand.GetStructureKey(uri1), DedupeCommand.GetStructureKey(uri2));
    }

    [Fact]
    public void GetStructureKey_DifferentStructure()
    {
        var uri1 = new Uri("https://example.com/users/1");
        var uri2 = new Uri("https://example.com/posts/1");

        Assert.NotEqual(DedupeCommand.GetStructureKey(uri1), DedupeCommand.GetStructureKey(uri2));
    }

    [Fact]
    public void GetStructureKey_DifferentQueryParams()
    {
        var uri1 = new Uri("https://example.com/users?sort=name");
        var uri2 = new Uri("https://example.com/users?filter=active");

        Assert.NotEqual(DedupeCommand.GetStructureKey(uri1), DedupeCommand.GetStructureKey(uri2));
    }
}
