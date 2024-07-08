using System.Text;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using Serilog;
using Serilog.Core;

namespace CSharpLocalAndRemote.Storage;

public class TenistasStorageCsv : ITenistasStorage
{
    private readonly Logger logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();

    public async Task<Result<List<Tenista>, TenistaError.StorageError>> ImportAsync(FileInfo file)
    {
        logger.Debug("Importando fichero de csv {file}", file.FullName);
        return await ReadLinesAsync(file);
    }

    public async Task<Result<int, TenistaError.StorageError>> ExportAsync(FileInfo file, List<Tenista> data)
    {
        logger.Debug("Exportando fichero de csv {file}", file.FullName);

        return await file.EnsureFileCanExists()
            .Match(
                async f =>
                {
                    try
                    {
                        // Escibimos la cabecera del fichero de forma asíncrona
                        await File.WriteAllTextAsync(f.FullName,
                            "id,nombre,pais,altura,peso,puntos,mano,fechaNacimiento,createdAt,updatedAt,deletedAt,isDeleted\n");
                        // Convertimos los tenistas a la representación en cadena y los escribimos de manera asíncrona
                        var tenistaDtos = data.Select(tenista => tenista.ToTenistaDto());
                        foreach (var tenistaDto in tenistaDtos)
                            await File.AppendAllTextAsync(f.FullName,
                                $"{tenistaDto.Id},{tenistaDto.Nombre},{tenistaDto.Pais},{tenistaDto.Altura},{tenistaDto.Peso},{tenistaDto.Puntos},{tenistaDto.Mano},{tenistaDto.FechaNacimiento},{tenistaDto.CreatedAt},{tenistaDto.UpdatedAt},{tenistaDto.IsDeleted}\n");
                        // Retornamos el resultado exitoso con la cantidad de tenistas exportados
                        return Result.Success<int, TenistaError.StorageError>(data.Count);
                    }
                    catch (Exception ex)
                    {
                        return Result.Failure<int, TenistaError.StorageError>(
                            new TenistaError.StorageError(
                                $"No se puede escribir en el fichero {file.FullName}: {ex.Message}"));
                    }
                },
                error => Task.FromResult(Result.Failure<int, TenistaError.StorageError>(error)));
    }

    private async Task<Result<List<Tenista>, TenistaError.StorageError>> ReadLinesAsync(FileInfo file)
    {
        if (!File.Exists(file.FullName))
            return Result.Failure<List<Tenista>, TenistaError.StorageError>(
                new TenistaError.StorageError($"El fichero no existe {file.FullName}"));
        var lines = await File.ReadAllLinesAsync(file.FullName, Encoding.UTF8);
        var tenistas = lines
            .Skip(1)
            .Select(line => line.Split(',')) // Dividimos la línea en celdas
            .Select(fila => fila.Select(item => item.Trim()).ToArray()) // Limpiamos los espacios en blanco
            .Select(item => parseLine(item)) // Parseamos cada línea a un objeto Tenista
            .ToList();
        return Result.Success<List<Tenista>, TenistaError.StorageError>(tenistas);
    }

    private Tenista parseLine(string[] parts)
    {
        logger.Debug("Parseando línea {item}", parts);
        return new TenistaDto(
            long.Parse(parts[0]),
            parts[1],
            parts[2],
            int.Parse(parts[3]),
            int.Parse(parts[4]),
            int.Parse(parts[5]),
            parts[6],
            parts[7],
            parts.Length > 8 ? parts[8] : null,
            parts.Length > 9 ? parts[9] : null,
            parts.Length > 10 && bool.Parse(parts[10])
        ).ToTenista();
    }
}