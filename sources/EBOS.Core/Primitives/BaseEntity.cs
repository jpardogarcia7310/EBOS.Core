using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.Core.Primitives;

/// <summary>
/// Practical basic class for entities.
/// The entities in the domain must inherit from this class.
/// </summary>
public abstract class BaseEntity 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
}
