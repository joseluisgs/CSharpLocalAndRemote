using System.Text;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;

namespace CSharpLocalAndRemote.Storage;

public class TenistasStorageJson : ITenistasStorage
{
    private readonly Logger logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();

    public async Task<Result<List<Tenista>, TenistaError.StorageError>> ImportAsync(FileInfo file)
    {
        logger.Debug("Importando fichero JSON {file}", file.FullName);
        return await ReadLinesAsync(file);
    }

    public async Task<Result<int, TenistaError.StorageError>> ExportAsync(FileInfo file, List<Tenista> data)
    {
        logger.Debug("Exportando fichero JSON {file}", file.FullName);

        return await file.EnsureFileCanExists()
            .Match(
                async f =>
                {
                    try
                    {
                        var tenistaDtos = data.Select(tenista => tenista.ToTenistaDto()).ToList();
                        var json = JsonConvert.SerializeObject(tenistaDtos, Formatting.Indented);
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
            var tenistasDto = JsonConvert.DeserializeObject<List<TenistaDto>>(json) ?? [];
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