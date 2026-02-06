namespace EBOS.Core.Primitives.Interfaces;

/// <summary>
/// Marks an entity that supports soft deletion.
/// </summary>
public interface ISoftDeletable
{
    bool Erased { get; set; }
}
