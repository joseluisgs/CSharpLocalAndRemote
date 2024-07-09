using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace CSharpLocalAndRemote.Rest;

public class RefitClient
{
    public static ITenistasApiRest CreateClient(string baseUrl)
    {
        var settings = new RefitSettings
        {
            // Serializador de contenido JSON con Newtonsoft.Json
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
            {
                ContractResolver =
                    new CamelCasePropertyNamesContractResolver(), // CamelCase en JSON para evitar propiedades con guiones bajos
                NullValueHandling = NullValueHandling.Ignore // Ignorar valores nulos
            })
        };

        return RestService.For<ITenistasApiRest>(baseUrl, settings);
    }
}