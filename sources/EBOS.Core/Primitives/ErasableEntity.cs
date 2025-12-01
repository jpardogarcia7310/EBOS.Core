using EBOS.Core.Primitives.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EBOS.Core.Primitives;

/// <summary>
/// Clase base práctica para las entidades que soportan borrado lógico.
/// </summary>
public abstract class ErasableEntity : BaseEntity, ISoftDeletable
{
    [Required]
    public bool Erased { get; set; }
}
