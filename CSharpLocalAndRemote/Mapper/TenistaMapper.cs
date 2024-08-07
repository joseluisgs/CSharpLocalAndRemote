﻿using System.Globalization;
using CSharpLocalAndRemote.Database;
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
            dto.Nombre,
            dto.Pais,
            dto.Altura,
            dto.Peso,
            dto.Puntos,
            Enum.Parse<Mano>(dto.Mano,
                true), // Se utiliza Enum.Parse para convertir el string a su correspondiente enumerado
            DateTime.Parse(dto.FechaNacimiento, null,
                DateTimeStyles
                    .RoundtripKind), // Se utiliza DateTime.Parse para convertir el string a DateTime con el formato correcto ISO
            dto.CreatedAt != null
                ? DateTime.Parse(dto.CreatedAt, null, DateTimeStyles.RoundtripKind)
                : DateTime.Now, // Se utiliza DateTime.Parse para: DateTime.Now,
            dto.UpdatedAt != null
                ? DateTime.Parse(dto.UpdatedAt, null, DateTimeStyles.RoundtripKind)
                : DateTime.Now, // Se utiliza DateTime.Parse para: DateTime.Now,
            dto.IsDeleted ?? false,
            dto.Id
        );
    }

    // Conversión de Tenista a TenistaDto
    public static TenistaDto ToTenistaDto(this Tenista tenista)
    {
        return new TenistaDto(
            tenista.Id,
            tenista.Nombre,
            tenista.Pais,
            tenista.Altura,
            tenista.Peso,
            tenista.Puntos,
            tenista.Mano.ToString().ToUpper(),
            tenista.FechaNacimiento
                .ToString("yyyy-MM-dd"), // Se utiliza ToString con el formato correcto ISO
            tenista.CreatedAt.ToString("o"), // Se utiliza ToString con el formato correcto ISO
            tenista.UpdatedAt.ToString("o"),
            tenista.IsDeleted
        );
    }

    public static Tenista ToTenista(this TenistaEntity entity)
    {
        return new Tenista(
            entity.Nombre,
            entity.Pais,
            entity.Altura,
            entity.Peso,
            entity.Puntos,
            Enum.Parse<Mano>(entity.Mano,
                true),
            DateTime.ParseExact(entity.FechaNacimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateTime.Parse(entity.CreatedAt, null, DateTimeStyles.RoundtripKind),
            DateTime.Parse(entity.UpdatedAt, null, DateTimeStyles.RoundtripKind),
            entity.IsDeleted,
            entity.Id
        );
    }

    public static TenistaEntity ToTenistaEntity(this Tenista tenista)
    {
        // Como no tiene constructor con parámetros, se crea un objeto y se asignan los valores uno por uno
        return new TenistaEntity
        {
            Id = tenista.Id,
            Nombre = tenista.Nombre,
            Pais = tenista.Pais,
            Altura = tenista.Altura,
            Peso = tenista.Peso,
            Puntos = tenista.Puntos,
            Mano = tenista.Mano.ToString().ToUpper(),
            FechaNacimiento = tenista.FechaNacimiento.ToString("yyyy-MM-dd"),
            CreatedAt = tenista.CreatedAt.ToString("o"),
            UpdatedAt = tenista.UpdatedAt.ToString("o"),
            IsDeleted = tenista.IsDeleted
        };
    }
}