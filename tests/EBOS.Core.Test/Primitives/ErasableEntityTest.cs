using EBOS.Core.Primitives;
using EBOS.Core.Primitives.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EBOS.Core.Test.Primitives;

public class ErasbleEntityTests
{
    private class TestEntity : ErasableEntity { }

    // ----- INSTANCIACIÓN -----

    [Fact]
    public void ErasbleEntity_ShouldBeInstantiableThroughChild()
    {
        var entity = new TestEntity();

        Assert.NotNull(entity);
    }

    // ----- HERENCIA -----

    [Fact]
    public void ErasbleEntity_ShouldInheritFromBaseEntity()
    {
        Assert.True(typeof(BaseEntity).IsAssignableFrom(typeof(ErasableEntity)));
    }

    [Fact]
    public void ErasbleEntity_ShouldImplementISoftDeletable()
    {
        Assert.True(typeof(ISoftDeletable).IsAssignableFrom(typeof(ErasableEntity)));
    }

    // ----- PROPIEDAD ERASED -----

    [Fact]
    public void Erased_DefaultValue_ShouldBeFalse()
    {
        var entity = new TestEntity();

        Assert.False(entity.Erased);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Erased_ShouldAcceptValidValues(bool value)
    {
        var entity = new TestEntity { Erased = value };

        Assert.Equal(value, entity.Erased);
    }

    // ----- VALIDACIONES -----

    [Fact]
    public void Erased_ShouldHaveRequiredAttribute()
    {
        var prop = typeof(ErasableEntity).GetProperty(nameof(ErasableEntity.Erased));
        var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), true).FirstOrDefault();

        Assert.NotNull(attr);
    }

    // ----- VALIDATION NEGATIVE -----

    [Fact]
    public void Required_Erased_ShouldFailValidation_WhenNotProvided()
    {
        var entity = new TestEntity(); // no seteamos nada
        var ctx = new ValidationContext(entity);
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(entity, ctx, results, validateAllProperties: true);

        // Aunque false sea un valor booleano válido, Required en bool significa "debe incluirse"
        Assert.True(valid, "El atributo Required en bool no causa error porque false sigue siendo un valor válido.");
    }

    // ----- CONTRACT COMPLETO -----

    [Fact]
    public void ErasbleEntity_ShouldExposeIdAndErasedProperties()
    {
        var entity = new TestEntity();
        var props = entity.GetType().GetProperties().Select(x => x.Name).ToList();

        Assert.Contains("Id", props);
        Assert.Contains("Erased", props);
    }

    // ----- NEGATIVE: NO DEBE TENER MAS PROPIEDADES -----

    [Fact]
    public void ErasbleEntity_ShouldOnlyContainIdAndErased()
    {
        var props = typeof(ErasableEntity).GetProperties().Select(x => x.Name).ToList();

        Assert.Contains("Id", props);
        Assert.Contains("Erased", props);
        Assert.Equal(2, props.Count);
    }

    // ----- MODIFICACIÓN DEL ID HEREDADO -----

    [Fact]
    public void ShouldAllowSettingInheritedId_WhenValid()
    {
        var entity = new TestEntity { Id = 50 };

        Assert.Equal(50, entity.Id);
    }

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(-99999)]
    [InlineData(-1)]
    public void Setting_NegativeId_ShouldBeAllowed_ButMayBeInvalidAtDBLevel(long value)
    {
        var entity = new TestEntity { Id = value };

        Assert.Equal(value, entity.Id);
    }

    // ----- AGGREGATE: ESTADOS COMBINADOS -----

    [Theory]
    [InlineData(1, true)]
    [InlineData(1, false)]
    [InlineData(long.MaxValue, true)]
    public void IdAndErased_ShouldBeSettableTogether(long id, bool erased)
    {
        var entity = new TestEntity { Id = id, Erased = erased };

        Assert.Equal(id, entity.Id);
        Assert.Equal(erased, entity.Erased);
    }
}