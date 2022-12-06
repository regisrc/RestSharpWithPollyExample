using RestSharp;

namespace Application.Api.Settings
{
    public class ApiSettings
    {
        public RestClient RestClient { get; set; }

        public Method Method { get; set; }

        public string? Path { get; set; }

        public string? Auth { get; set; }

        public HeadersSettings Headers { get; set; }

        public ParamsSettings Params { get; set; }
    }
}
