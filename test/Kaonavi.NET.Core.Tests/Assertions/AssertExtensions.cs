using System.Diagnostics.CodeAnalysis;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests.Assertions;

[ShouldlyMethods]
internal static class AssertExtensions
{
    public static void ShouldBeCalled(this Mock<HttpMessageHandler> handler, Times times, params Action<HttpRequestMessage>[] conditions)
    {
        if (conditions is null || conditions.Length == 0)
            handler.VerifyAnyRequest(times);
        else
            handler.VerifyRequest(req => { req.ShouldSatisfyAllConditions(conditions); return true; }, times);
    }

    public static void ShouldBeCalledOnce(this Mock<HttpMessageHandler> handler, params Action<HttpRequestMessage>[] conditions)
        => handler.ShouldBeCalled(Times.Once(), conditions);

    public static void ShouldNotBeCalled(this Mock<HttpMessageHandler> handler, params Action<HttpRequestMessage>[] conditions)
        => handler.ShouldBeCalled(Times.Never(), conditions);

    public static void ShouldHaveJsonBody(this HttpContent content, [StringSyntax(StringSyntaxAttribute.Json)] string expectedJson, string? customMessage = null)
    {
        using var actual = JsonDocument.Parse(content.ReadAsStream());
        using var expected = JsonDocument.Parse(expectedJson);
        if (!JsonElement.DeepEquals(actual.RootElement, expected.RootElement))
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expectedJson, actual.RootElement, customMessage).ToString());
    }

    public static void ShouldHaveJsonBody(this HttpContent content, ReadOnlySpan<byte> utf8PropertyName, [StringSyntax(StringSyntaxAttribute.Json)] string expectedJson, string? customMessage = null)
    {
        using var actual = JsonDocument.Parse(content.ReadAsStream());
        using var expected = JsonDocument.Parse(expectedJson);
        if (!JsonElement.DeepEquals(actual.RootElement.GetProperty(utf8PropertyName), expected.RootElement))
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expectedJson, actual.RootElement, customMessage).ToString());
    }
}
