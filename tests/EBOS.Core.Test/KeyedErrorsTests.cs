using EBOS.Core.Primitives;

namespace EBOS.Core.Test;

public class KeyedErrorsTests
{
    [Fact]
    public void TryAdd_AddsWhenNotExists()
    {
        var keyed = new KeyedErrors();
        var e = new ErrorResult("m", 10);
        var added = keyed.TryAdd(e);
        Assert.True(added);
        Assert.True(keyed.ContainsKey(10));
    }

    [Fact]
    public void TryAdd_ReturnsFalseWhenDuplicate()
    {
        var keyed = new KeyedErrors
        {
            new ErrorResult("a", 1)
        };
        var added = keyed.TryAdd(new ErrorResult("b", 1));
        Assert.False(added);
        Assert.Single(keyed);
    }

    [Fact]
    public void TryAdd_ThrowsOnNull()
    {
        var keyed = new KeyedErrors();
        Assert.Throws<ArgumentNullException>(() => keyed.TryAdd(null!));
    }

    [Fact]
    public void AddOrUpdate_ReplacesExisting()
    {
        var keyed = new KeyedErrors
        {
            new ErrorResult("a", 1)
        };
        keyed.AddOrUpdate(new ErrorResult("b", 1));
        Assert.Single(keyed);
        Assert.Equal("b", keyed[1].Message);
    }

    [Fact]
    public void AddOrUpdate_ThrowsOnNull()
    {
        var keyed = new KeyedErrors();
        Assert.Throws<ArgumentNullException>(() => keyed.AddOrUpdate(null!));
    }

    [Fact]
    public void ContainsKey_WorksForExistingAndNotExisting()
    {
        var keyed = new KeyedErrors
        {
            new ErrorResult("a", 5)
        };
        Assert.True(keyed.ContainsKey(5));
        Assert.False(keyed.ContainsKey(99));
    }
}
