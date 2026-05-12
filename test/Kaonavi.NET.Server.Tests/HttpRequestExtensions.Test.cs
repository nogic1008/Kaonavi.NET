using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Kaonavi.Net.Server.Tests;

/// <summary>
/// <see cref="HttpRequestExtensions"/>の単体テスト
/// </summary>
[Category("Server")]
public sealed class HttpRequestExtensionsTest
{
    /// <summary>
    /// Content-Typeが"application/json"でない場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="false"/>を返す。
    /// </summary>
    /// <param name="contentType">Content-Type</param>
    [Test]
    [DisplayName(
        $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)}(request) > Content-Typeが $contentType の場合、falseを返す。"
    )]
    [Arguments("text/plain")]
    [Arguments("applicaton/xml")]
    [Arguments("application/x-www-form-urlencoded")]
    public async Task When_ContentType_Is_Not_ApplicationJson_IsKaonaviWebhookRequest_Returns_False(
        string contentType
    )
    {
        // Arrange
        var request = HttpRequest.Mock();
        request.ContentType.Returns(contentType);

        // Act - Assert
        await Assert
            .That(HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, "token"))
            .IsFalse();
    }

    /// <summary>
    /// User-Agentが"Kaonavi-Webhook"を含まない場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="false"/>を返す。
    /// </summary>
    [Test]
    [DisplayName(
        $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)}(request) > User-Agent が $userAgent の場合、falseを返す。"
    )]
    [Arguments(null)]
    [Arguments("Mozilla")]
    public async Task When_UserAgent_Is_Not_KaonaviWebhook_IsKaonaviWebhookRequest_Returns_False(
        string? userAgent
    )
    {
        // Arrange
        var request = HttpRequest.Mock();
        request.ContentType.Returns("application/json");
        var headers = IHeaderDictionary.Mock();
        headers.UserAgent.Returns(new StringValues(userAgent));
        request.Headers.Returns(headers.Object);

        // Act - Assert
        await Assert
            .That(HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, "token"))
            .IsFalse();
    }

    /// <summary>
    /// Headerに"Kaonavi-Token"が含まれない場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="false"/>を返す。
    /// </summary>
    [Test]
    [DisplayName(
        $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)}(request) > Header に Kaonavi-Token が含まれない場合、falseを返す。"
    )]
    public async Task When_Header_Does_Not_Contain_KaonaviToken_IsKaonaviWebhookRequest_Returns_False()
    {
        // Arrange
        var request = HttpRequest.Mock();
        request.ContentType.Returns("application/json");
        var headers = IHeaderDictionary.Mock();
        headers.UserAgent.Returns(new StringValues("Kaonavi-Webhook"));
        headers.TryGetValue("Kaonavi-Token").Returns(false);
        request.Headers.Returns(headers.Object);

        // Act - Assert
        await Assert
            .That(HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, "token"))
            .IsFalse();
    }

    /// <summary>
    /// Headerの"Kaonavi-Token"に指定したトークンが含まれない場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="false"/>を返す。
    /// </summary>
    [Test]
    [DisplayName(
        $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)}(request) > Header の Kaonavi-Token に指定したトークンが含まれない場合、falseを返す。"
    )]
    public async Task When_Header_Does_Not_Contain_Specified_Token_IsKaonaviWebhookRequest_Returns_False()
    {
        // Arrange
        var request = HttpRequest.Mock();
        request.ContentType.Returns("application/json");
        var headers = IHeaderDictionary.Mock();
        headers.UserAgent.Returns(new StringValues("Kaonavi-Webhook"));
        headers.TryGetValue("Kaonavi-Token").Returns(true).SetsOutValue("another-token");
        request.Headers.Returns(headers.Object);

        // Act - Assert
        await Assert
            .That(HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, "token"))
            .IsFalse();
    }

    /// <summary>
    /// Headerの"Kaonavi-Token"に指定したトークンが含まれる場合、<see cref="HttpRequestExtensions.IsKaonaviWebhookRequest"/>は<see langword="true"/>を返す。
    /// </summary>
    [Test]
    [DisplayName(
        $"{nameof(HttpRequestExtensions.IsKaonaviWebhookRequest)}(request) > Header の Kaonavi-Token に指定したトークンが含まれる場合、trueを返す。"
    )]
    public async Task When_Header_Contains_Specified_Token_IsKaonaviWebhookRequest_Returns_True()
    {
        // Arrange
        const string token = "token";
        var request = HttpRequest.Mock();
        request.ContentType.Returns("application/json");
        var headers = IHeaderDictionary.Mock();
        headers.UserAgent.Returns(new StringValues("Kaonavi-Webhook"));
        headers.TryGetValue("Kaonavi-Token").Returns(true).SetsOutValue(token);
        request.Headers.Returns(headers.Object);

        // Act - Assert
        await Assert
            .That(HttpRequestExtensions.IsKaonaviWebhookRequest(request.Object, token))
            .IsTrue();
    }
}
