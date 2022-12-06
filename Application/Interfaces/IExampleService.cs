using RestSharp;

namespace Application.Interfaces
{
    public interface IExampleService
    {
        Task<object?> Request(string body, RestClient? restClient = null);
    }
}
