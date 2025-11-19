using EBOS.Core.Extensions;

namespace EBOS.Core.Test.Extensions;

public class ArrayExtensionsTest
{
    [Fact]
    public void Add_TargetIsNull_ThrowsArgumentNullException()
    {
        int[]? target = null;

        var ex = Assert.Throws<ArgumentNullException>(() => target!.Add(1));

        Assert.Equal("target", ex.ParamName);
    }

    [Fact]
    public void Add_Append_AddsItemAtEndAndPreservesExistingElements()
    {
        var target = new[] { 1, 2, 3 };

        var result = target.Add(4);

        Assert.Equal([1, 2, 3, 4], result);
        Assert.NotSame(target, result);
    }

    [Fact]
    public void Add_PrependTrue_AddsItemAtStartAndPreservesExistingElements()
    {
        var target = new[] { 1, 2, 3 };

        var result = target.Add(0, prepend: true);

        Assert.Equal([0, 1, 2, 3], result);
        Assert.NotSame(target, result);
    }

    [Fact]
    public void Add_Append_OnEmptyArray_ReturnsArrayWithSingleItem()
    {
        var target = Array.Empty<int>();

        var result = target.Add(5);

        Assert.Equal([5], result);
    }

    [Fact]
    public void Add_PrependTrue_OnEmptyArray_ReturnsArrayWithSingleItem()
    {
        var target = Array.Empty<int>();

        var result = target.Add(5, prepend: true);

        Assert.Equal([5], result);
    }

    [Fact]
    public void Add_ReferenceType_Append_AddsItemAtEnd()
    {
        var target = new[] { "a", "b" };

        var result = target.Add("c");

        Assert.Equal(expectedArray, result);
    }

    private static readonly string[] expected = ["z", "a", "b"];
    private static readonly string[] expectedArray = ["a", "b", "c"];

    [Fact]
    public void Add_ReferenceType_Prepend_AddsItemAtStart()
    {
        var target = new[] { "a", "b" };

        var result = target.Add("z", prepend: true);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_CanAppendNullItem_ForReferenceType()
    {
        var target = new[] { "a" };

        var result = target.Add(null);

        Assert.Equal(2, result.Length);
        Assert.Equal("a", result[0]);
        Assert.Null(result[1]);
    }

    [Fact]
    public void Add_DoesNotModifyOriginalArray_WhenAppending()
    {
        var target = new[] { 1, 2, 3 };
        var originalCopy = (int[])target.Clone();

        var result = target.Add(4);

        Assert.Equal(originalCopy, target);
        Assert.Equal([1, 2, 3, 4], result);
    }

    [Fact]
    public void Add_DoesNotModifyOriginalArray_WhenPrepending()
    {
        var target = new[] { 1, 2, 3 };
        var originalCopy = (int[])target.Clone();

        var result = target.Add(0, prepend: true);

        Assert.Equal(originalCopy, target);
        Assert.Equal(new[] { 0, 1, 2, 3 }, result);
    }
}