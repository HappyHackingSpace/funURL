using funURL.CLI.Core;

namespace funURL.CLI.Tests;

public class ResultTests
{
    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Failure_CreatesFailedResult()
    {
        var result = Result<int>.Failure("Something went wrong");

        Assert.False(result.IsSuccess);
        Assert.Equal("Something went wrong", result.Error);
    }

    [Fact]
    public void Value_ThrowsOnFailure()
    {
        var result = Result<int>.Failure("Error");

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Error_ThrowsOnSuccess()
    {
        var result = Result<int>.Success(42);

        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void Map_TransformsSuccessValue()
    {
        var result = Result<int>.Success(42);
        var mapped = result.Map(x => x * 2);

        Assert.True(mapped.IsSuccess);
        Assert.Equal(84, mapped.Value);
    }

    [Fact]
    public void Map_PreservesFailure()
    {
        var result = Result<int>.Failure("Error");
        var mapped = result.Map(x => x * 2);

        Assert.False(mapped.IsSuccess);
        Assert.Equal("Error", mapped.Error);
    }

    [Fact]
    public void Map_CanChangeType()
    {
        var result = Result<int>.Success(42);
        var mapped = result.Map(x => x.ToString());

        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value);
    }

    [Fact]
    public void Bind_ChainsSuccessfulOperations()
    {
        var result = Result<int>.Success(42);
        var bound = result.Bind(x => Result<string>.Success(x.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("42", bound.Value);
    }

    [Fact]
    public void Bind_ShortCircuitsOnFailure()
    {
        var result = Result<int>.Failure("Initial error");
        var bound = result.Bind(x => Result<string>.Success(x.ToString()));

        Assert.False(bound.IsSuccess);
        Assert.Equal("Initial error", bound.Error);
    }

    [Fact]
    public void Bind_PropagatesSecondFailure()
    {
        var result = Result<int>.Success(42);
        var bound = result.Bind(x => Result<string>.Failure("Second error"));

        Assert.False(bound.IsSuccess);
        Assert.Equal("Second error", bound.Error);
    }
}
