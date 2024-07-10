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

    [Required] [MaxLength(100)] public string Nombre { get; set; } = string.Empty;

    [Required] [MaxLength(50)] public string Pais { get; set; } = string.Empty;

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

    [Required] public string Mano { get; set; } = string.Empty;

    [Required] public string FechaNacimiento { get; set; } = string.Empty;

    [Required] public string CreatedAt { get; set; } = string.Empty;

    [Required] public string UpdatedAt { get; set; } = string.Empty;

    [DefaultValue(false)] public bool IsDeleted { get; set; }

    public override string ToString()
    {
        return
            $"Tenista(Id: {Id}, Nombre: {Nombre}, Pais: {Pais}, Altura: {Altura}, Peso: {Peso}, Puntos: {Puntos}, Mano: {Mano}, Fecha Nacimiento: {FechaNacimiento}, Created At: {CreatedAt}, Updated At: {UpdatedAt}, Is Deleted: {IsDeleted})";
    }
}