using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RandomFixtureKit;

namespace Kaonavi.Net.Server.Tests;

/// <summary>
/// <see cref="HttpRequestExtensions"/>の単体テスト
/// </summary>
[TestCategory("Server")]
[TestClass]
public sealed class HttpRequestExtensionsTest
{
    /// <summary>
    /// Content-Typeが"application/json"でない場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="false"/>を返す。
    /// </summary>
    /// <param name="contentType">Content-Type</param>
    [TestMethod($"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > Content-Typeが\"applicaton/json\"でない場合、falseを返す。")]
    [DataRow("text/plain", DisplayName = $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > Content-Typeが\"text/plain\"の場合、falseを返す。")]
    [DataRow("applicaton/xml", DisplayName = $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > Content-Typeが\"applicaton/xml\"の場合、falseを返す。")]
    [DataRow("application/x-www-form-urlencoded", DisplayName = $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > Content-Typeが\"application/x-www-form-urlencoded\"の場合、falseを返す。")]
    public void When_ContentType_Is_Not_ApplicationJson_IsKaonaviWebhookRequest_Returns_False(string contentType)
    {
        // Arrange
        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.ContentType).Returns(contentType);

        // Act - Assert
        HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, "token").ShouldBeFalse();
    }

    /// <summary>
    /// User-Agentが"Kaonavi-Webhook"を含まない場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="false"/>を返す。
    /// </summary>
    [TestMethod($"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > User-Agentが\"Kaonavi-Webhook\"でない場合、falseを返す。")]
    [DataRow(null, DisplayName = $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > User-Agentが未設定の場合、falseを返す。")]
    [DataRow("Mozilla", DisplayName = $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > User-Agentが\"Mozilla\"の場合、falseを返す。")]
    public void When_UserAgent_Is_Not_KaonaviWebhook_IsKaonaviWebhookRequest_Returns_False(string? userAgent)
    {
        // Arrange
        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.ContentType).Returns("application/json");
        request.SetupGet(x => x.Headers.UserAgent).Returns(new StringValues(userAgent));

        // Act - Assert
        HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, "token").ShouldBeFalse();
    }

    /// <summary>
    /// Headerに"Kaonavi-Token"が含まれない場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="false"/>を返す。
    /// </summary>
    [TestMethod($"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > Headerに\"Kaonavi-Token\"が含まれない場合、falseを返す。")]
    public void When_Header_Does_Not_Contain_KaonaviToken_IsKaonaviWebhookRequest_Returns_False()
    {
        // Arrange
        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.ContentType).Returns("application/json");
        request.SetupGet(x => x.Headers.UserAgent).Returns(new StringValues("Kaonavi-Webhook"));
        request.Setup(x => x.Headers.TryGetValue("Kaonavi-Token", out It.Ref<StringValues>.IsAny)).Returns(false);

        // Act - Assert
        HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, "token").ShouldBeFalse();
    }

    /// <summary>
    /// Headerの"Kaonavi-Token"に指定したトークンが含まれない場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="false"/>を返す。
    /// </summary>
    [TestMethod($"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > Headerの\"Kaonavi-Token\"に指定したトークンが含まれない場合、falseを返す。")]
    public void When_Header_Does_Not_Contain_Specified_Token_IsKaonaviWebhookRequest_Returns_False()
    {
        // Arrange
        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.ContentType).Returns("application/json");
        request.SetupGet(x => x.Headers.UserAgent).Returns(new StringValues("Kaonavi-Webhook"));
        request.Setup(x => x.Headers.TryGetValue("Kaonavi-Token", out It.Ref<StringValues>.IsAny))
            .Callback(MockCallback)
            .Returns(true);

        // Act - Assert
        HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, "token").ShouldBeFalse();

        static void MockCallback(string key, out StringValues values)
            => values = new StringValues("another-token");
    }

    /// <summary>
    /// Headerの"Kaonavi-Token"に指定したトークンが含まれる場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="true"/>を返す。
    /// </summary>
    [TestMethod($"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)} > Headerの\"Kaonavi-Token\"に指定したトークンが含まれる場合、trueを返す。")]
    public void When_Header_Contains_Specified_Token_IsKaonaviWebhookRequest_Returns_True()
    {
        // Arrange
        string token = FixtureFactory.Create<string>();
        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.ContentType).Returns("application/json");
        request.SetupGet(x => x.Headers.UserAgent).Returns(new StringValues("Kaonavi-Webhook"));
        request.Setup(x => x.Headers.TryGetValue("Kaonavi-Token", out It.Ref<StringValues>.IsAny))
            .Callback(MockCallback)
            .Returns(true);

        // Act - Assert
        HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, token).ShouldBeTrue();

        void MockCallback(string key, out StringValues values)
            => values = new StringValues(token);
    }
}
