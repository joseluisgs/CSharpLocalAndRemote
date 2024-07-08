namespace CSharpLocalAndRemote.Database;

public class TenistaEntity
{
    public long Id { get; set; }
    public string Nombre { get; set; }
    public string Pais { get; set; }
    public int Altura { get; set; }
    public int Peso { get; set; }
    public int Puntos { get; set; }
    public string Mano { get; set; }
    public string FechaNacimiento { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}