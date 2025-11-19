using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class ISoftDeleteTests
{
    #region Helper implementation
    private class SoftDeleteEntity : ISoftDelete
    {
        public bool Erased { get; set; }
    }
    #endregion

    #region Interface shape (reflection)
    [Fact]
    public void ISoftDelete_HasProperty_Erased_WithGetAndSet_OfTypeBool()
    {
        var type = typeof(ISoftDelete);
        var prop = type.GetProperty(nameof(ISoftDelete.Erased));

        Assert.NotNull(prop);
        Assert.Equal(typeof(bool), prop!.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }
    #endregion

    #region Basic contract behavior via implementation
    [Fact]
    public void Implementation_DefaultErasedValue_IsFalseByDefault()
    {
        ISoftDelete entity = new SoftDeleteEntity();

        Assert.False(entity.Erased);
    }

    [Fact]
    public void Implementation_CanSetErasedToTrue()
    {
        ISoftDelete entity = new SoftDeleteEntity
        {
            Erased = true
        };

        Assert.True(entity.Erased);
    }

    [Fact]
    public void Implementation_CanToggleErasedFlag()
    {
        ISoftDelete entity = new SoftDeleteEntity
        {
            Erased = true
        };

        Assert.True(entity.Erased);

        entity.Erased = false;

        Assert.False(entity.Erased);
    }
    #endregion
}