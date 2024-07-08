using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSharpLocalAndRemote.Database;

[Table("TenistaEntity")]
public class TenistaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] public string Nombre { get; set; }

    [Required] public string Pais { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Altura debe ser mayor o igual a 0")]
    public int Altura { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Peso debe ser mayor o igual a 0")]
    public int Peso { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Puntos debe ser mayor o igual a 0")]
    [DefaultValue(0)]
    public int Puntos { get; set; }

    [Required] public string Mano { get; set; }

    [Required] public string FechaNacimiento { get; set; }

    [Required] public string CreatedAt { get; set; }

    [Required] public string UpdatedAt { get; set; }

    [DefaultValue(false)] public bool IsDeleted { get; set; }
}