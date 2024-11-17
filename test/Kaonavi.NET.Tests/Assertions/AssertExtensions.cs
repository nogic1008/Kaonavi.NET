namespace Kaonavi.Net.Tests.Assertions;

internal static class AssertExtensions
{
    public static HttpRequestMessageAssertions Should(this HttpRequestMessage instance) => new(instance);
    public static JsonElementAssertions Should(this JsonElement instance) => new(instance);
}
