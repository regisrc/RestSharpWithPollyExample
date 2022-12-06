using Application.Api.Services;
using Application.Api.Settings;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Net;

namespace Application.Services
{
    public class ExampleService : ApiService, IExampleService
    {
        public ExampleService(ILogger<ApiService> logger) : base(logger)
        {
        }

        public override RestClient CreateRestClient(string url, RestClient? restClient)
        {
            return restClient ?? new RestClient(url);
        }

        public async Task<object?> Request(string body, RestClient? restClient = null)
        {
            var apiSettings = new ApiSettings
            {
                RestClient = CreateRestClient("https://viacep.com.br/", restClient),
                Method = Method.Get,
                Path = "ws/01001000/json/",
                Headers = new HeadersSettings(new List<DataSettingsBase>
                {
                    new DataSettingsBase("Teste", "Teste")
                }),
                Params = new ParamsSettings(new List<DataSettingsBase>{
                    new DataSettingsBase("Teste", "Teste")
                })
            };

            var pollySettings = new PollySettings
            {
                RetryCondition = (x => x.StatusCode == HttpStatusCode.BadRequest),
                RetryCount = 5,
                DelaySeconds = 10
            };

            var result = await PrepareRequest(apiSettings, pollySettings, body);

            if (result.StatusCode == HttpStatusCode.BadRequest)
                _logger.LogError("Teste");

            return result.Content;
        }

    }
}
