using EBOS.Core.Extensions;

namespace EBOS.Core.Test.Extensions;

public class IListExtensionTests
{
    #region Null collection
    [Fact]
    public void IsNullOrEmpty_NullCollection_ReturnsTrue()
    {
        IList<int>? collection = null;
        var result = collection.IsNullOrEmpty();

        Assert.True(result);
    }
    #endregion

    #region Empty collection
    [Fact]
    public void IsNullOrEmpty_EmptyListOfValueType_ReturnsTrue()
    {
        IList<int> collection = new List<int>();
        var result = collection.IsNullOrEmpty();

        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_EmptyListOfReferenceType_ReturnsTrue()
    {
        IList<string> collection = new List<string>();
        var result = collection.IsNullOrEmpty();

        Assert.True(result);
    }
    #endregion

    #region Non-empty collection
    [Fact]
    public void IsNullOrEmpty_ListWithSingleItemValueType_ReturnsFalse()
    {
        IList<int> collection = new List<int> { 1 };
        var result = collection.IsNullOrEmpty();

        Assert.False(result);
    }

    [Fact]
    public void IsNullOrEmpty_ListWithMultipleItemsValueType_ReturnsFalse()
    {
        IList<int> collection = new List<int> { 1, 2, 3 };
        var result = collection.IsNullOrEmpty();

        Assert.False(result);
    }

    [Fact]
    public void IsNullOrEmpty_ListWithSingleItemReferenceType_ReturnsFalse()
    {
        IList<string> collection = new List<string> { "item" };
        var result = collection.IsNullOrEmpty();

        Assert.False(result);
    }

    [Fact]
    public void IsNullOrEmpty_ListWithMultipleItemsReferenceType_ReturnsFalse()
    {
        IList<string> collection = new List<string> { "a", "b" };
        var result = collection.IsNullOrEmpty();

        Assert.False(result);
    }

    [Fact]
    public void IsNullOrEmpty_ListWithNullReferenceItemsButNonEmpty_ReturnsFalse()
    {
        IList<string?> collection = new List<string?> { null, null };
        var result = collection.IsNullOrEmpty();

        Assert.False(result);
    }
    #endregion
}