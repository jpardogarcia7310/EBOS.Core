using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.Core.Primitives;

/// <summary>
/// Clase base práctica para las entidades.
/// Las entidades del dominio deben heredar de esta clase.
/// </summary>
public abstract class BaseEntity 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
}
