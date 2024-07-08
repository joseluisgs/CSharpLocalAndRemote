using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Error;

namespace CSharpLocalAndRemote.Storage;

public static class Utils
{
    public static Result<FileInfo, TenistaError.StorageError> EnsureFileCanExists(this FileInfo file)
    {
        try
        {
            if (file.Exists) return Result.Success<FileInfo, TenistaError.StorageError>(file);
            // Si no existe debemos ser capaces de crearlo
            file.Create().Dispose();
            // Si no se ha lanzado ninguna excepción, devolvemos el fichero
            return Result.Success<FileInfo, TenistaError.StorageError>(file);
        }
        catch (IOException e)
        {
            return Result.Failure<FileInfo, TenistaError.StorageError>(
                new TenistaError.StorageError($"No se puede acceder al fichero {file.FullName}: {e.Message}"));
        }
        catch (UnauthorizedAccessException e)
        {
            return Result.Failure<FileInfo, TenistaError.StorageError>(
                new TenistaError.StorageError(
                    $"No tiene permisos para acceder al fichero {file.FullName}: {e.Message}"));
        }
    }
}