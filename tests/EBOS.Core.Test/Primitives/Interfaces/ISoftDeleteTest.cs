using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class ISoftDeletableTests
{
    private class Dummy : ISoftDeletable
    {
        public bool Erased { get; set; }
    }

    // ----- INSTANTIATION -----

    [Fact]
    public void SoftDeletable_ShouldAllowInstantiationViaImplementer()
    {
        ISoftDeletable entity = new Dummy();

        Assert.NotNull(entity);
    }

    // ----- DEFAULT -----

    [Fact]
    public void Erased_DefaultValue_ShouldBeFalse()
    {
        var entity = new Dummy();

        Assert.False(entity.Erased);
    }

    // ----- READ/WRITE -----

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Erased_ShouldAcceptBooleanValues(bool value)
    {
        var entity = new Dummy { Erased = value };

        Assert.Equal(value, entity.Erased);
    }

    // ----- CONTRACT CHECK -----

    [Fact]
    public void ISoftDeletable_ShouldHaveBooleanErasedProperty()
    {
        var prop = typeof(ISoftDeletable).GetProperty("Erased");

        Assert.NotNull(prop);
        Assert.Equal(typeof(bool), prop!.PropertyType);
    }
}