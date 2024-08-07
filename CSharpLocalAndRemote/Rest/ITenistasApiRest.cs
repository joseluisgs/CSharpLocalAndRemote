﻿using CSharpLocalAndRemote.Dto;
using Refit;

namespace CSharpLocalAndRemote.Rest;

public interface ITenistasApiRest
{
    const string ApiRestUrl = "https://my-json-server.typicode.com/joseluisgs/KotlinLocalAndRemote/";

    [Get("/tenistas")]
    Task<List<TenistaDto>> GetAllAsync();

    [Get("/tenistas/{id}")]
    Task<TenistaDto> GetByIdAsync(long id);

    [Post("/tenistas")]
    Task<TenistaDto> SaveAsync([Body] TenistaDto tenista);

    [Put("/tenistas/{id}")]
    Task<TenistaDto> UpdateAsync(long id, [Body] TenistaDto tenista);

    [Delete("/tenistas/{id}")]
    Task DeleteAsync(long id);
}