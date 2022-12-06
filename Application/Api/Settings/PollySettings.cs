using RestSharp;

namespace Application.Api.Settings
{
    public class PollySettings
    {
        public Func<RestResponse<string>, bool> RetryCondition { get; set; }

        public int RetryCount { get; set; }

        public double DelaySeconds { get; set; }
    }
}
