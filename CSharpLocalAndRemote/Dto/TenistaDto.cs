using Newtonsoft.Json;

namespace CSharpLocalAndRemote.Dto;

public record TenistaDto(
    long Id,
    string Nombre,
    string Pais,
    int Altura,
    int Peso,
    int Puntos,
    string Mano,
    [JsonProperty("fecha_nacimiento")] string FechaNacimiento,
    [JsonProperty("created_at")] string? CreatedAt = null,
    [JsonProperty("updated_at")] string? UpdatedAt = null,
    [JsonProperty("is_deleted")] bool? IsDeleted = false
);