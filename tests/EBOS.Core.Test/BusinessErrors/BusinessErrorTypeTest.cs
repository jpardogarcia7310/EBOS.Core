using EBOS.Core.BusinessErrors;

namespace EBOS.Core.Test.BusinessErrors;

public class BusinessErrorTypeTests
{
    [Fact]
    public void BusinessErrorType_IsEnum()
    {
        Assert.True(typeof(BusinessErrorType).IsEnum);
    }

    [Fact]
    public void BusinessErrorType_UnderlyingType_IsInt()
    {
        var underlying = Enum.GetUnderlyingType(typeof(BusinessErrorType));

        Assert.Equal(typeof(int), underlying);
    }

    [Fact]
    public void BusinessErrorType_ContainsAllDefinedNames()
    {
        var names = Enum.GetNames<BusinessErrorType>();

        Assert.Contains(nameof(BusinessErrorType.ClassificationDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.WasteTypeDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.ManagerDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.DestinationDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.OriginDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.SubOriginDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.ColorDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.IconDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.WasteCenterDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.ContainerCenterDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.OriginWasteCenterDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.DestinationWasteCenterDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.ManagerWasteCenterDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.ConfigurationDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.CenterDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.WeighingDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.WeighingMachineDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.DeliveryNoteDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.ValorizationDontWork), names);
        Assert.Contains(nameof(BusinessErrorType.ObjectiveDontWork), names);
        Assert.Equal(20, names.Length);
    }

    [Fact]
    public void BusinessErrorType_Values_AreUnique()
    {
        var values = Enum.GetValues(typeof(BusinessErrorType)).Cast<int>().ToArray();
        var distinct = values.Distinct().ToArray();

        Assert.Equal(values.Length, distinct.Length);
    }

    [Fact]
    public void BusinessErrorType_CanParseFromValidString()
    {
        var parsed = Enum.Parse<BusinessErrorType>("ClassificationDontWork");

        Assert.Equal(BusinessErrorType.ClassificationDontWork, parsed);
    }

    [Fact]
    public void BusinessErrorType_TryParse_InvalidString_ReturnsFalse()
    {
        var success = Enum.TryParse<BusinessErrorType>("NotExistingValue", out var result);

        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void BusinessErrorType_FirstValue_IsClassificationDontWork_AndZero()
    {
        var first = BusinessErrorType.ClassificationDontWork;

        Assert.Equal(0, (int)first);
    }

    [Fact]
    public void BusinessErrorType_LastValue_IsObjectiveDontWork()
    {
        var last = BusinessErrorType.ObjectiveDontWork;

        Assert.Equal(19, (int)last);
    }
}
