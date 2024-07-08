using System.Globalization;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Mapper;

/*
 * En este archivo se definen dos métodos de extensión para convertir objetos de tipo Tenista a TenistaDto y viceversa u otras.
 * Esto permite convertir objetos de un dominio a otro y viceversa, manteniendo la integridad de la información.
 * Son métodos de extensión porque se definen en una clase estática y se utilizan como si fueran métodos de las clases Tenista, TenistaDto y otras.
 * por eso necesitamos la palabra clave this en el primer parámetro de los métodos.
 */
public static class TenistaMapper
{
    // Conversión de TenistaDto a Tenista
    public static Tenista ToTenista(this TenistaDto dto)
    {
        return new Tenista(
            nombre: dto.Nombre,
            pais: dto.Pais,
            altura: dto.Altura,
            peso: dto.Peso,
            puntos: dto.Puntos,
            mano: Enum.Parse<Mano>(dto.Mano,
                ignoreCase: true), // Se utiliza Enum.Parse para convertir el string a su correspondiente enumerado
            fechaNacimiento: DateTime.Parse(dto.FechaNacimiento, null,
                DateTimeStyles
                    .RoundtripKind), // Se utiliza DateTime.Parse para convertir el string a DateTime con el formato correcto ISO
            createdAt: dto.CreatedAt != null
                ? DateTime.Parse(dto.CreatedAt, null, DateTimeStyles.RoundtripKind)
                : DateTime.Now, // Se utiliza DateTime.Parse para: DateTime.Now,
            updatedAt: dto.UpdatedAt != null
                ? DateTime.Parse(dto.UpdatedAt, null, DateTimeStyles.RoundtripKind)
                : DateTime.Now, // Se utiliza DateTime.Parse para: DateTime.Now,
            isDeleted: dto.IsDeleted ?? false,
            id: dto.Id
        );
    }

    // Conversión de Tenista a TenistaDto
    public static TenistaDto ToTenistaDto(this Tenista tenista)
    {
        return new TenistaDto(
            Id: tenista.Id,
            Nombre: tenista.Nombre,
            Pais: tenista.Pais,
            Altura: tenista.Altura,
            Peso: tenista.Peso,
            Puntos: tenista.Puntos,
            Mano: tenista.Mano.ToString(),
            FechaNacimiento: tenista.FechaNacimiento
                .ToString("yyyy-MM-dd"), // Se utiliza ToString con el formato correcto ISO
            CreatedAt: tenista.CreatedAt.ToString("o"), // Se utiliza ToString con el formato correcto ISO
            UpdatedAt: tenista.UpdatedAt.ToString("o"),
            IsDeleted: tenista.IsDeleted
        );
    }
}