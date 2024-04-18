namespace Kaonavi.Net.Tests;

public static class HttpRequestMessageExtensions
{
    public static HttpRequestMessageAssertions Should(this HttpRequestMessage instance) => new(instance);
}
