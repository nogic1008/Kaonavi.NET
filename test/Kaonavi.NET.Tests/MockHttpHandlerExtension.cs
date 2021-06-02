using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Moq.Contrib.HttpClient;
using Moq.Language.Flow;

namespace Kaonavi.Net.Tests
{
    public static class MockHttpHandlerExtension
    {
        private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);
        public static IReturnsResult<HttpMessageHandler> ReturnsJson<T>(
            this ISetup<HttpMessageHandler, Task<HttpResponseMessage>> setup,
            T content,
            HttpStatusCode statusCode = HttpStatusCode.OK)
            => setup.ReturnsResponse(statusCode, JsonSerializer.Serialize(content, _options), "application/json");
    }
}
