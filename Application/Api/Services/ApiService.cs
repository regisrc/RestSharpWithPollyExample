using Application.Api.Interfaces;
using Application.Api.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RestSharp;

namespace Application.Api.Services
{
    public abstract class ApiService : IApiService
    {
        protected readonly ILogger<ApiService> _logger;

        protected ApiService(ILogger<ApiService> logger)
        {
            _logger = logger;
        }

        public abstract RestClient CreateRestClient(string url, RestClient? restClient = null);

        public async Task<RestResponse<string>> PrepareRequest(ApiSettings apiSettings, PollySettings pollySettings, object body)
        {
            var request = new RestRequest(apiSettings.Path);

            SetAuthenticationHeader(request, apiSettings.Auth);
            SetBody(request, body);
            SetHeaders(request, apiSettings.Headers.Headers);
            SetParams(request, apiSettings.Params.Params);

            var policyResponse = await GetPolicyResponse(apiSettings, pollySettings, request);

            var response = GetPolicyResult(policyResponse);

            var isSuccessful = response.IsSuccessful ? "Yes" : "No";

            _logger.LogInformation($"Status code: {response.StatusCode} Success: {isSuccessful}");

            return response;
        }

        public virtual void SetParams(RestRequest request, List<DataSettingsBase> @params)
        {
            if (!@params.Any())
                return;

            @params.ForEach(p => request.AddParameter(p.Key, p.Value));
        }

        public virtual void SetHeaders(RestRequest request, List<DataSettingsBase> headers)
        {
            if (!headers.Any())
                return;

            headers.ForEach(h => request.AddHeader(h.Key, h.Value));
        }

        public virtual void SetBody(RestRequest request, object body)
        {
            var messageBody = PrepareMessageToJsonBody(body);

            request.AddJsonBody(messageBody);
        }

        public virtual string PrepareMessageToJsonBody(object message)
        {
            return JToken.FromObject(message).ToString(Formatting.None);
        }

        public virtual async Task<PolicyResult<RestResponse<string>>> GetPolicyResponse(
            ApiSettings apiSettings,
            PollySettings pollySettings,
            RestRequest request)
        {
            var retryPolicy = Policy.HandleResult(pollySettings.RetryCondition)
                .WaitAndRetryAsync(pollySettings.RetryCount, delay => TimeSpan.FromSeconds(pollySettings.DelaySeconds));

            return await retryPolicy.ExecuteAndCaptureAsync(async () => await apiSettings.RestClient.ExecuteAsync<string>(request, apiSettings.Method));
        }

        public virtual RestResponse<string> GetPolicyResult(PolicyResult<RestResponse<string>> policyResult) =>
            policyResult.Result == null ? policyResult.FinalHandledResult : policyResult.Result;

        public virtual void SetAuthenticationHeader(RestRequest request, string? authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader))
                return;

            var authenticationHeaderName = "Authorization";

            request.AddHeader(authenticationHeaderName, authorizationHeader);
        }
    }
}
