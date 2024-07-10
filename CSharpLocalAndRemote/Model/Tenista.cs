namespace CSharpLocalAndRemote.model;

public class Tenista
{
    public const long NewId = 0L;

    // Constructor
    public Tenista(
        string nombre,
        string pais,
        int altura,
        int peso,
        int puntos,
        Mano mano,
        DateTime fechaNacimiento,
        DateTime? createdAt = null, // ? -> Nullable 
        DateTime? updatedAt = null,
        bool isDeleted = false,
        long id = NewId
    )
    {
        Id = id;
        Nombre = nombre;
        Pais = pais;
        Altura = altura;
        Peso = peso;
        Puntos = puntos;
        Mano = mano;
        FechaNacimiento = fechaNacimiento;
        CreatedAt = createdAt ?? DateTime.Now; // Si no hay valor, se toma el actual tiempo
        UpdatedAt = updatedAt ?? DateTime.Now;
        IsDeleted = isDeleted;
    }

    public long Id { get; set; }
    public string Nombre { get; set; }
    public string Pais { get; set; }
    public int Altura { get; set; }
    public int Peso { get; set; }
    public int Puntos { get; set; }
    public Mano Mano { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public override string ToString()
    {
        return
            $"Tenista(Id: {Id}, Nombre: {Nombre}, Pais: {Pais}, Altura: {Altura}, Peso: {Peso}, Puntos: {Puntos}, Mano: {Mano}, Fecha Nacimiento: {FechaNacimiento.ToString("yyyy-MM-dd")}, Created At: {CreatedAt.ToString("o")}, Updated At: {UpdatedAt.ToString("o")}, Is Deleted: {IsDeleted})";
    }
}

public enum Mano
{
    Diestro,
    Zurdo
}