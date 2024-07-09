using System.Text;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Logger;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CSharpLocalAndRemote.Storage;

public class TenistasStorageJson : ITenistasStorage
{
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasStorageJson>.GetLogger();

    public async Task<Result<List<Tenista>, TenistaError.StorageError>> ImportAsync(FileInfo file)
    {
        _logger.Debug("Importando fichero JSON {file}", file.FullName);
        return await ReadLinesAsync(file);
    }

    public async Task<Result<int, TenistaError.StorageError>> ExportAsync(FileInfo file, List<Tenista> data)
    {
        _logger.Debug("Exportando fichero JSON {file}", file.FullName);

        return await file.EnsureFileCanExists()
            .Match(
                async f =>
                {
                    try
                    {
                        var tenistaDtos = data.Select(tenista => tenista.ToTenistaDto()).ToList();

                        // Settings del JSON
                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            NullValueHandling = NullValueHandling.Ignore,
                            Formatting = Formatting.Indented // Incluye esta línea para mantener el formato adecuado
                        };

                        var json = JsonConvert.SerializeObject(tenistaDtos, settings);
                        await File.WriteAllTextAsync(f.FullName, json, Encoding.UTF8);
                        return Result.Success<int, TenistaError.StorageError>(data.Count);
                    }
                    catch (Exception ex)
                    {
                        return Result.Failure<int, TenistaError.StorageError>(
                            new TenistaError.StorageError($"No se puede escribir el fichero JSON: {ex.Message}"));
                    }
                },
                error => Task.FromResult(Result.Failure<int, TenistaError.StorageError>(error)));
    }


    private async Task<Result<List<Tenista>, TenistaError.StorageError>> ReadLinesAsync(FileInfo file)
    {
        if (!File.Exists(file.FullName))
            return Result.Failure<List<Tenista>, TenistaError.StorageError>(
                new TenistaError.StorageError($"El fichero no existe {file.FullName}"));

        try
        {
            var json = await File.ReadAllTextAsync(file.FullName, Encoding.UTF8);

            // Opcion de deserializar el JSON
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var tenistasDto = JsonConvert.DeserializeObject<List<TenistaDto>>(json, settings) ?? [];
            var tenistas = tenistasDto.Select(dto => dto.ToTenista()).ToList();
            return Result.Success<List<Tenista>, TenistaError.StorageError>(tenistas);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<Tenista>, TenistaError.StorageError>(
                new TenistaError.StorageError($"No se puede leer el fichero JSON: {ex.Message}"));
        }
    }
}