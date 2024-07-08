namespace CSharpLocalAndRemote.Dto;

public record TenistaDto(
    long Id, 
    string Nombre, 
    string Pais, 
    int Altura, 
    int Peso, 
    int Puntos, 
    string Mano, 
    string FechaNacimiento,
    string? CreatedAt = null,
    string? UpdatedAt = null,
    bool? IsDeleted = false
);