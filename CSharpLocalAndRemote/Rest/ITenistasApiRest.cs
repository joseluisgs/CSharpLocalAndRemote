using CSharpLocalAndRemote.Dto;
using Refit;

namespace CSharpLocalAndRemote.Rest;

public interface ITenistasApiRest
{
    [Get("/tenistas")]
    Task<List<TenistaDto>> GetAll();

    [Get("/tenistas/{id}")]
    Task<TenistaDto> GetById(long id);

    [Post("/tenistas")]
    Task<TenistaDto> Save([Body] TenistaDto tenista);

    [Put("/tenistas/{id}")]
    Task<TenistaDto> Update(long id, [Body] TenistaDto tenista);

    [Delete("/tenistas/{id}")]
    Task Delete(long id);
}