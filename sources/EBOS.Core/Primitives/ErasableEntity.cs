using EBOS.Core.Primitives.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EBOS.Core.Primitives;

/// <summary>
/// Practical base class for entities that support logical deletion.
/// </summary>
public abstract class ErasableEntity : BaseEntity, ISoftDeletable
{
    [Required]
    public bool Erased { get; set; }
}
