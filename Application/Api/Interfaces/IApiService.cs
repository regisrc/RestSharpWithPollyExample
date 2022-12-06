using Application.Api.Settings;
using RestSharp;

namespace Application.Api.Interfaces
{
    public interface IApiService
    { 
        Task<RestResponse<string>> PrepareRequest(ApiSettings apiSettings, PollySettings pollySettings, object body);
    }
}
