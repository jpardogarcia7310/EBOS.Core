namespace EBOS.Core.Primitives.Interfaces;

/// <summary>
/// Marca una entidad que soporta borrado lógico.
/// </summary>
public interface ISoftDeletable
{
    bool Erased { get; set; }
}