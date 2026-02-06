using EBOS.Core.Primitives;
using EBOS.Core.Primitives.Interfaces;
using System.Collections.ObjectModel;

namespace EBOS.Core.Test;

public class OperationResultTests
{
    [Fact]
    public void NewOperationResult_HasNoErrors()
    {
        var op = new OperationResult<int>();
        Assert.False(op.HasErrors);
        Assert.Empty(op.Errors);
    }

    [Fact]
    public void AddError_ByObject_AddsError()
    {
        var op = new OperationResult<string>();
        var err = new ErrorResult("msg", 1, null);
        op.AddError(err);
        Assert.True(op.HasErrors);
        Assert.Single(op.Errors);
        Assert.Equal(1, op.Errors.First().Code);
    }

    [Fact]
    public void AddError_ByComponents_AddsError()
    {
        var op = new OperationResult<object>();
        op.AddError("failure", 42, "ex");
        Assert.True(op.HasErrors);
        var e = op.Errors.First();
        Assert.Equal(42, e.Code);
        Assert.Equal("failure", e.Message);
        Assert.Equal("ex", e.ExceptionMsg);
    }

    [Fact]
    public void AddError_ByObject_ThrowsOnNull()
    {
        var op = new OperationResult<string>();
        Assert.Throws<ArgumentNullException>(() => op.AddError(null!));
    }

    [Fact]
    public void AddErrors_AddsMultiple()
    {
        var op = new OperationResult<object>();
        var list = new[] { new ErrorResult("a", 1), new ErrorResult("b", 2) };
        op.AddErrors(list);
        Assert.Equal(2, op.Errors.Count);
    }

    [Fact]
    public void AddErrors_ThrowsOnNull()
    {
        var op = new OperationResult<object>();
        Assert.Throws<ArgumentNullException>(() => op.AddErrors(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void AddError_ByComponents_ThrowsOnInvalidMessage(string message)
    {
        var op = new OperationResult<object>();
        Assert.Throws<ArgumentException>(() => op.AddError(message, 1, null));
    }

    [Fact]
    public void AddException_AddsErrorFromException()
    {
        var op = new OperationResult<object>();
        var ex = new InvalidOperationException("boom");
        op.AddException(ex, code: -5);
        Assert.True(op.HasErrors);
        var e = op.Errors.First();
        Assert.Equal(-5, e.Code);
        Assert.Contains("boom", e.Message);
        Assert.NotNull(e.ExceptionMsg);
    }

    [Fact]
    public void AddException_ThrowsOnNull()
    {
        var op = new OperationResult<object>();
        Assert.Throws<ArgumentNullException>(() => op.AddException(null!));
    }

    [Fact]
    public void ClearErrors_RemovesAll()
    {
        var op = new OperationResult<int>();
        op.AddError("x", 1);
        op.ClearErrors();
        Assert.False(op.HasErrors);
        Assert.Empty(op.Errors);
    }

    [Fact]
    public void ToKeyedCollection_ReplacesDuplicates_KeepingLast()
    {
        var op = new OperationResult<int>();
        op.AddError("first", 1);
        op.AddError("second", 1); // same code
        var keyed = op.ToKeyedCollection();
        Assert.Single(keyed);
        var item = keyed[1];
        Assert.Equal("second", item.Message);
    }

    [Fact]
    public void ToCollection_ReturnsMutableCopy()
    {
        var op = new OperationResult<int>();
        op.AddError("a", 1);
        var col = op.ToCollection();
        Assert.IsType<Collection<IErrorResult>>(col);
        col.Add(new ErrorResult("b", 2));
        Assert.Equal(2, col.Count);
        // original remains unchanged
        Assert.Single(op.Errors);
    }

    [Fact]
    public void ToReadOnlyCollection_ReturnsReadOnlyCopy()
    {
        var op = new OperationResult<int>();
        op.AddError("a", 1);
        var col = op.ToReadOnlyCollection();

        Assert.True(((ICollection<IErrorResult>)col).IsReadOnly);
        Assert.Single(col);
    }
}
