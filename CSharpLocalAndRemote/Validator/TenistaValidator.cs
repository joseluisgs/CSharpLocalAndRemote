using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Validator;

public static class TenistaValidator
{
    public static Result<Tenista, TenistaError.ValidationError> Validate(this Tenista tenista)
    {
        if (string.IsNullOrWhiteSpace(tenista.Nombre))
            return Result.Failure<Tenista, TenistaError.ValidationError>(
                new TenistaError.ValidationError("El nombre es requerido"));

        if (tenista.Altura <= 0)
            return Result.Failure<Tenista, TenistaError.ValidationError>(
                new TenistaError.ValidationError("La altura debe ser mayor a 0"));

        if (tenista.Peso <= 0)
            return Result.Failure<Tenista, TenistaError.ValidationError>(
                new TenistaError.ValidationError("El peso debe ser mayor o igual a 0"));

        if (tenista.Puntos < 0)
            return Result.Failure<Tenista, TenistaError.ValidationError>(
                new TenistaError.ValidationError("Los puntos deben ser mayor o igual a 0"));

        if (tenista.FechaNacimiento > DateTime.Now)
            return Result.Failure<Tenista, TenistaError.ValidationError>(
                new TenistaError.ValidationError("La fecha de nacimiento no puede ser mayor a la fecha actual"));

        return Result.Success<Tenista, TenistaError.ValidationError>(tenista);
    }
}