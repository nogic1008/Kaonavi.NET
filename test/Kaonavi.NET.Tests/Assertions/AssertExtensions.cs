namespace Kaonavi.Net.Tests.Assertions;

public static class HttpRequestMessageExtensions
{
    public static HttpRequestMessageAssertions Should(this HttpRequestMessage instance) => new(instance);
}

public static class JsonElementExtensions
{
    public static JsonElementAssertions Should(this JsonElement instance) => new(instance);
}
