using EBOS.Core.Extensions;
using System.Data;

namespace EBOS.Core.Test.Extensions;

public class IEnumerableExtensionsTest
{
    #region Helper classes
    private class SimpleClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class NullablePropertiesClass
    {
        public int? Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    private interface INoPropertiesClass
    { }

    private class NoPropertiesClass : INoPropertiesClass
    { }

    private interface IHasProperties
    {
        int Id { get; set; }
        string Description { get; set; }
    }

    private class HasPropertiesClass : IHasProperties
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
    }
    #endregion

    #region Null data
    [Fact]
    public void ToDataTable_DataIsNull_ThrowsArgumentNullException()
    {
        IEnumerable<SimpleClass>? data = null;
        var ex = Assert.Throws<ArgumentNullException>(() => data!.ToDataTable());

        Assert.Equal("data", ex.ParamName);
    }
    #endregion

    #region Simple class mapping
    [Fact]
    public void ToDataTable_SimpleClass_BuildsColumnsAndRowsCorrectly()
    {
        var data = new List<SimpleClass>
        {
            new() { Id = 1, Name = "Alice" },
            new() { Id = 2, Name = "Bob" }
        };
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.Equal(2, table.Columns.Count);
        Assert.True(table.Columns.Contains(nameof(SimpleClass.Id)));
        Assert.True(table.Columns.Contains(nameof(SimpleClass.Name)));
        Assert.Equal(typeof(int), table.Columns[nameof(SimpleClass.Id)]!.DataType);
        Assert.Equal(typeof(string), table.Columns[nameof(SimpleClass.Name)]!.DataType);
        Assert.Equal(2, table.Rows.Count);
        Assert.Equal(1, table.Rows[0][nameof(SimpleClass.Id)]);
        Assert.Equal("Alice", table.Rows[0][nameof(SimpleClass.Name)]);
        Assert.Equal(2, table.Rows[1][nameof(SimpleClass.Id)]);
        Assert.Equal("Bob", table.Rows[1][nameof(SimpleClass.Name)]);
    }

    [Fact]
    public void ToDataTable_EmptyEnumerable_ReturnsTableWithColumnsButNoRows()
    {
        var data = new List<SimpleClass>();
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.Equal(2, table.Columns.Count);
        Assert.Equal(0, table.Rows.Count);
    }
    #endregion

    #region Nullable properties
    [Fact]
    public void ToDataTable_NullableProperties_UsesUnderlyingTypesForColumns()
    {
        var data = new List<NullablePropertiesClass>
        {
            new()
            {
                Quantity = 10,
                CreatedAt = new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Unspecified)
            }
        };
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.True(table.Columns.Contains(nameof(NullablePropertiesClass.Quantity)));
        Assert.True(table.Columns.Contains(nameof(NullablePropertiesClass.CreatedAt)));
        Assert.Equal(typeof(int), table.Columns[nameof(NullablePropertiesClass.Quantity)]!.DataType);
        Assert.Equal(typeof(DateTime), table.Columns[nameof(NullablePropertiesClass.CreatedAt)]!.DataType);
        Assert.Equal(1, table.Rows.Count);
        Assert.Equal(10, table.Rows[0][nameof(NullablePropertiesClass.Quantity)]);
        Assert.Equal(
            new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Unspecified),
            table.Rows[0][nameof(NullablePropertiesClass.CreatedAt)]
        );
    }

    [Fact]
    public void ToDataTable_NullableProperties_WithNullValues_StoresDBNull()
    {
        var data = new List<NullablePropertiesClass>
        {
            new()
            {
                Quantity = null,
                CreatedAt = null
            }
        };
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.Equal(1, table.Rows.Count);
        Assert.Equal(DBNull.Value, table.Rows[0][nameof(NullablePropertiesClass.Quantity)]);
        Assert.Equal(DBNull.Value, table.Rows[0][nameof(NullablePropertiesClass.CreatedAt)]);
    }
    #endregion

    #region Type with no properties
    [Fact]
    public void ToDataTable_TypeWithNoProperties_HasNoColumnsButRowsAreAdded()
    {
        var data = new List<INoPropertiesClass>
        {
            new NoPropertiesClass(),
            new NoPropertiesClass()
        };
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.Empty(table.Columns);
        Assert.Equal(2, table.Rows.Count);
    }

    [Fact]
    public void ToDataTable_EmptyEnumerableOfTypeWithNoProperties_ReturnsEmptySchemaAndNoRows()
    {
        var data = new List<INoPropertiesClass>();
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.Empty(table.Columns);
        Assert.Equal(0, table.Rows.Count);
    }
    #endregion

    #region Primitive types
    [Fact]
    public void ToDataTable_PrimitiveTypeEnumerable_HasNoColumnsAndCreatesRows()
    {
        var data = new[] { 1, 2, 3 };
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.Empty(table.Columns);
        Assert.Equal(3, table.Rows.Count);
    }
    #endregion

    #region Interface with properties
    [Fact]
    public void ToDataTable_InterfaceWithProperties_UsesInterfacePropertiesForSchema()
    {
        var data = new List<IHasProperties>
        {
            new HasPropertiesClass { Id = 1, Description = "First" },
            new HasPropertiesClass { Id = 2, Description = "Second" }
        };
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.True(table.Columns.Contains(nameof(IHasProperties.Id)));
        Assert.True(table.Columns.Contains(nameof(IHasProperties.Description)));
        Assert.Equal(typeof(int), table.Columns[nameof(IHasProperties.Id)]!.DataType);
        Assert.Equal(typeof(string), table.Columns[nameof(IHasProperties.Description)]!.DataType);
        Assert.Equal(2, table.Rows.Count);
        Assert.Equal(1, table.Rows[0][nameof(IHasProperties.Id)]);
        Assert.Equal("First", table.Rows[0][nameof(IHasProperties.Description)]);
        Assert.Equal(2, table.Rows[1][nameof(IHasProperties.Id)]);
        Assert.Equal("Second", table.Rows[1][nameof(IHasProperties.Description)]);
    }

    #endregion

    #region Collections with null items

    [Fact]
    public void ToDataTable_EnumerableWithNullItem_CreatesRowWithDBNullValuesForNullItem()
    {
        var data = new List<SimpleClass?>
        {
            new() { Id = 1, Name = "Valid" },
            null
        };
        DataTable table = data.ToDataTable();

        Assert.NotNull(table);
        Assert.Equal(2, table.Rows.Count);
        Assert.Equal(1, table.Rows[0][nameof(SimpleClass.Id)]);
        Assert.Equal("Valid", table.Rows[0][nameof(SimpleClass.Name)]);
        Assert.Equal(DBNull.Value, table.Rows[1][nameof(SimpleClass.Id)]);
        Assert.Equal(DBNull.Value, table.Rows[1][nameof(SimpleClass.Name)]);
    }

    #endregion

    #region Data consistency

    [Fact]
    public void ToDataTable_RepeatedCallOnSameType_ProducesSameSchema()
    {
        var data1 = new List<SimpleClass>
        {
            new() { Id = 1, Name = "First" }
        };
        var data2 = new List<SimpleClass>
        {
            new() { Id = 2, Name = "Second" }
        };
        DataTable table1 = data1.ToDataTable();
        DataTable table2 = data2.ToDataTable();

        Assert.Equal(table1.Columns.Count, table2.Columns.Count);
        Assert.Equal(
            table1.Columns[nameof(SimpleClass.Id)]!.DataType,
            table2.Columns[nameof(SimpleClass.Id)]!.DataType
        );
        Assert.Equal(
            table1.Columns[nameof(SimpleClass.Name)]!.DataType,
            table2.Columns[nameof(SimpleClass.Name)]!.DataType
        );
    }

    [Fact]
    public void ToDataTable_ItemsWithMixedNullsAndValues_AreMappedCorrectly()
    {
        var data = new List<NullablePropertiesClass>
        {
            new()
            {
                Quantity = 5,
                CreatedAt = null
            },
            new()
            {
                Quantity = null,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
            }
        };
        DataTable table = data.ToDataTable();

        Assert.Equal(2, table.Rows.Count);
        Assert.Equal(5, table.Rows[0][nameof(NullablePropertiesClass.Quantity)]);
        Assert.Equal(DBNull.Value, table.Rows[0][nameof(NullablePropertiesClass.CreatedAt)]);
        Assert.Equal(DBNull.Value, table.Rows[1][nameof(NullablePropertiesClass.Quantity)]);
        Assert.Equal(
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            table.Rows[1][nameof(NullablePropertiesClass.CreatedAt)]
        );
    }
    #endregion
}
