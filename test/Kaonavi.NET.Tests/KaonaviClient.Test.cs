using System.Text;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Kaonavi.Net.Tests.Assertions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Moq.Contrib.HttpClient;
using RandomFixtureKit;

namespace Kaonavi.Net.Tests;

/// <summary><see cref="KaonaviClient"/>の単体テスト</summary>
[TestClass]
public sealed partial class KaonaviClientTest
{
    /// <summary>テスト用の<see cref="HttpClient.BaseAddress"/></summary>
    private static readonly Uri _baseUri = new("https://example.com/");

    /// <summary><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></summary>
    /*lang=json,strict*/
    private const int TaskId = 1;

    /// <summary>タスク結果JSON</summary>
    /*lang=json,strict*/
    private const string TaskJson = """{"task_id":1}""";

    /// <summary>
    /// テスト対象(System Under Test)となる<see cref="KaonaviClient"/>のインスタンスを生成します。
    /// </summary>
    /// <param name="handler">HttpClientをモックするためのHandlerオブジェクト</param>
    /// <param name="accessToken">アクセストークン</param>
    /// <param name="timeProvider">TimeProvider</param>
    /// <param name="key">Consumer Key</param>
    /// <param name="secret">Consumer Secret</param>
    private static KaonaviClient CreateSut(Mock<HttpMessageHandler> handler, string? accessToken = null, TimeProvider? timeProvider = null, string key = "Key", string secret = "Secret")
    {
        var client = handler.CreateClient();
        client.BaseAddress = _baseUri;
        return new(client, key, secret, timeProvider ?? TimeProvider.System)
        {
            AccessToken = accessToken
        };
    }

    #region Constructor
    /// <summary>
    /// コンストラクターを呼び出す<see cref="Action"/>を生成します。
    /// </summary>
    /// <param name="client"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='client']"/></param>
    /// <param name="consumerKey"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerKey']"/></param>
    /// <param name="consumerSecret"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></param>
    private static Action Constructor(HttpClient? client, string? consumerKey, string? consumerSecret)
        => () => _ = new KaonaviClient(client!, consumerKey!, consumerSecret!);

    /// <summary>
    /// <paramref name="consumerKey"/>または<paramref name="consumerSecret"/>が<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    /// <param name="consumerKey"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerKey']"/></param>
    /// <param name="consumerSecret"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></param>
    /// <param name="paramName">例外の原因となったパラメータ名</param>
    [TestMethod($"{nameof(KaonaviClient)}(constructor) > {nameof(ArgumentNullException)}をスローする。"), TestCategory("Constructor")]
    [DataRow(null, "foo", "consumerKey", DisplayName = $"{nameof(KaonaviClient)}({nameof(HttpClient)}, null, \"foo\") > {nameof(ArgumentNullException)}(consumerKey)をスローする。")]
    [DataRow("foo", null, "consumerSecret", DisplayName = $"{nameof(KaonaviClient)}({nameof(HttpClient)}, \"foo\", null) > {nameof(ArgumentNullException)}(consumerSecret)をスローする。")]
    public void WhenParamIsNull_Constructor_Throws_ArgumentNullException(string? consumerKey, string? consumerSecret, string paramName)
        => Constructor(new(), consumerKey, consumerSecret).Should().ThrowExactly<ArgumentNullException>().WithParameterName(paramName);

    /// <summary>
    /// HttpClientが<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient)}(null, \"foo\", \"bar\") > ArgumentNullException(client)をスローする。"), TestCategory("Constructor")]
    public void WhenClientIsNull_Constructor_Throws_ArgumentNullException()
        => Constructor(null, "foo", "bar").Should().ThrowExactly<ArgumentNullException>().WithParameterName("client");

    /// <summary>
    /// TimeProviderが<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient)}({nameof(HttpClient)}, \"foo\", \"bar\", null) > ArgumentNullException(timeProvider)をスローする。"), TestCategory("Constructor")]
    public void WhenTimeProviderIsNull_Constructor_Throws_ArgumentNullException()
        => ((Action)(() => _ = new KaonaviClient(new(), "foo", "bar", null!)))
            .Should().ThrowExactly<ArgumentNullException>().WithParameterName("timeProvider");

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>のとき、既定値をセットする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient)}(constructor) > {nameof(HttpClient.BaseAddress)}がnullのとき、既定値をセットする。"), TestCategory("Constructor")]
    public void Constructor_Sets_BaseAddress_WhenIsNull()
    {
        // Arrange
        var client = new HttpClient();
        _ = client.BaseAddress.Should().BeNull();

        // Act
        _ = new KaonaviClient(client, "foo", "bar");

        // Assert
        _ = client.BaseAddress.Should().NotBeNull();
    }

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>でないときは、既定値をセットしない。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient)}(constructor) > {nameof(HttpClient.BaseAddress)}がnullでないときは、既定値をセットしない。"), TestCategory("Constructor")]
    public void Constructor_DoesNotSet_BaseAddress_WhenNotNull()
    {
        // Arrange
        var client = new HttpClient
        {
            BaseAddress = _baseUri
        };

        // Act
        _ = new KaonaviClient(client, "foo", "bar");

        // Assert
        _ = client.BaseAddress.Should().Be(_baseUri);
    }
    #endregion Constructor

    #region Properties
    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を返す。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient.AccessToken)}(get) > Kaonavi-Tokenヘッダーの値を返す。"), TestCategory("Properties")]
    public void AccessToken_Returns_Header_KaonaviToken_Value()
    {
        // Arrange
        var client = new HttpClient();
        string headerValue = FixtureFactory.Create<string>();
        client.DefaultRequestHeaders.Add("Kaonavi-Token", headerValue);

        // Act
        var sut = new KaonaviClient(client, "foo", "bar");

        // Assert
        _ = sut.AccessToken.Should().Be(headerValue);
    }

    /// <summary>
    /// Kaonavi-Tokenヘッダーがないとき、<see cref="KaonaviClient.AccessToken"/>は、<see langword="null"/>を返す。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient.AccessToken)}(get) > Kaonavi-Tokenヘッダーがないとき、nullを返す。"), TestCategory("Properties")]
    public void When_Header_KaonaviToken_IsNull_AccessToken_Returns_Null()
    {
        // Arrange - Act
        var sut = new KaonaviClient(new(), "foo", "bar");

        // Assert
        _ = sut.AccessToken.Should().BeNull();
    }

    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を設定する。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient.AccessToken)}(set) > Kaonavi-Tokenヘッダーの値を設定する。"), TestCategory("Properties")]
    public void AccessToken_Sets_Header_KaonaviToken()
    {
        // Arrange
        var client = new HttpClient();
        string headerValue = FixtureFactory.Create<string>();

        // Act
        _ = new KaonaviClient(client, "foo", "bar")
        {
            AccessToken = headerValue
        };

        // Assert
        _ = client.DefaultRequestHeaders.TryGetValues("Kaonavi-Token", out var values).Should().BeTrue();
        _ = values.Should().HaveCount(1).And.Contain(headerValue);
    }

    /// <summary>
    /// <see cref="KaonaviClient.UseDryRun"/>は、HttpClientのDry-Runヘッダーが"1"かどうかを返す。
    /// </summary>
    /// <param name="headerValue">Dry-Runヘッダーに設定する値</param>
    /// <param name="expected"><see cref="KaonaviClient.UseDryRun"/></param>
    [TestMethod($"{nameof(KaonaviClient.UseDryRun)}(get) > Dry-Run: 1 かどうかを返す。"), TestCategory("Properties")]
    [DataRow(null, false, DisplayName = $"{nameof(KaonaviClient.UseDryRun)} > Headerに Dry-Runの設定がない場合、 falseを返す。")]
    [DataRow("0", false, DisplayName = $"{nameof(KaonaviClient.UseDryRun)} > Headerに Dry-Run: 0 の設定がある場合、 falseを返す。")]
    [DataRow("1", true, DisplayName = $"{nameof(KaonaviClient.UseDryRun)} > Headerに Dry-Run: 1 の設定がある場合、 trueを返す。")]
    [DataRow("foo", false, DisplayName = $"{nameof(KaonaviClient.UseDryRun)} > Headerに Dry-Run: foo の設定がある場合、 falseを返す。")]
    public void UseDryRun_Returns_Header_DryRun_Is1(string? headerValue, bool expected)
    {
        // Arrange
        var client = new HttpClient();
        if (headerValue is not null)
            client.DefaultRequestHeaders.Add("Dry-Run", headerValue);

        // Act
        var sut = new KaonaviClient(client, "foo", "bar");

        // Assert
        _ = sut.UseDryRun.Should().Be(expected);
    }

    /// <summary>
    /// <see cref="KaonaviClient.UseDryRun"/>は、HttpClientのDry-Runヘッダー値を追加/削除する。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient.UseDryRun)}(set) > Dry-Runヘッダーを追加/削除する。"), TestCategory("Properties")]
    public void UseDryRun_Sets_Header_DryRun()
    {
        // Arrange
        var client = new HttpClient();

        #region UseDryRun = true
        // Act
        var sut = new KaonaviClient(client, "foo", "bar")
        {
            UseDryRun = true
        };

        // Assert
        _ = client.DefaultRequestHeaders.TryGetValues("Dry-Run", out var values).Should().BeTrue();
        _ = (values?.First().Should().Be("1"));
        #endregion UseDryRun = true

        #region UseDryRun = false
        // Act
        sut.UseDryRun = false;

        // Assert
        _ = client.DefaultRequestHeaders.TryGetValues("Dry-Run", out _).Should().BeFalse();
        #endregion UseDryRun = false
    }
    #endregion Properties

    #region API Common Path
    /// <summary>
    /// サーバー側がエラーを返したとき、<see cref="ApplicationException"/>の例外をスローする。
    /// </summary>
    /// <param name="statusCode">HTTPステータスコード</param>
    /// <param name="responseBody">エラー時のレスポンス本文</param>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="mediaType">MediaType</param>
    [TestMethod($"API Caller > ApplicationExceptionをスローする。"), TestCategory("API")]
    [DataRow(HttpStatusCode.Unauthorized, "application/json", /*lang=json,strict*/ """{"errors":["consumer_keyとconsumer_secretの組み合わせが不正です。"]}""", "consumer_keyとconsumer_secretの組み合わせが不正です。", DisplayName = $"API Caller > ApplicationExceptionをスローする。")]
    [DataRow(HttpStatusCode.TooManyRequests, "application/json", /*lang=json,strict*/ """{"errors":["1時間あたりのトークン発行可能数を超過しました。時間をおいてお試しください。"]}""", "1時間あたりのトークン発行可能数を超過しました。時間をおいてお試しください。", DisplayName = $"API Caller > ApplicationExceptionをスローする。")]
    [DataRow(HttpStatusCode.InternalServerError, "plain/text", "Error", "Error", DisplayName = $"API Caller > ApplicationExceptionをスローする。")]
    public async Task ApiCaller_Throws_ApplicationException(HttpStatusCode statusCode, string mediaType, string responseBody, string message)
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupAnyRequest()
            .ReturnsResponse(statusCode, responseBody, mediaType);

        // Act
        var sut = CreateSut(handler);
        var act = async () => await sut.AuthenticateAsync();

        // Assert
        _ = (await act.Should().ThrowExactlyAsync<ApplicationException>())
            .WithMessage(message)
            .WithInnerExceptionExactly<HttpRequestException>();
    }

    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>が<see langword="null"/>のとき、<see cref="KaonaviClient.AuthenticateAsync(CancellationToken)"/>を呼び出す。
    /// </summary>
    [TestMethod($"API Caller > {nameof(KaonaviClient.AuthenticateAsync)}を呼び出す。"), TestCategory("API")]
    public async Task When_AccessToken_IsNull_ApiCaller_Calls_AuthenticateAsync()
    {
        // Arrange
        string key = FixtureFactory.Create<string>();
        string secret = FixtureFactory.Create<string>();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.InternalServerError, "Error", "text/plain");

        // Act
        var sut = CreateSut(handler, key: key, secret: secret);
        var act = async () => await sut.Layout.ReadMemberLayoutAsync();

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ApplicationException>();

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Should().SendTo(HttpMethod.Post, "/token");

            // Header
            _ = (req.Headers.Authorization?.Scheme.Should().Be("Basic"));
            byte[] byteArray = Encoding.UTF8.GetBytes($"{key}:{secret}");
            string base64String = Convert.ToBase64String(byteArray);
            _ = (req.Headers.Authorization?.Parameter.Should().Be(base64String));

            // Body
            _ = req.Content.Should().BeAssignableTo<FormUrlEncodedContent>();
            string body = await req.Content!.ReadAsStringAsync();
            _ = body.Should().Be("grant_type=client_credentials");

            return true;
        }, Times.Once());
        handler.VerifyRequest(res => res.RequestUri?.PathAndQuery != "/token", Times.Never());
    }

    /// <summary>
    /// 更新リクエスト制限の対象となるAPIは、6回目の呼び出し前に1分間待機する。
    /// </summary>
    [TestMethod($"API Caller > 更新リクエスト制限の対象となるAPIは、6回目の呼び出し前に1分間待機する。"), TestCategory("API")]
    public async Task UpdateApi_Waits_UpdateLimit()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");
        var timeProvider = new FakeTimeProvider();

        // Act - Assert
        var sut = CreateSut(handler, "token", timeProvider);
        _ = sut.UpdateRequestCount.Should().Be(0);

        for (int i = 1; i <= 5; i++) // 1-5th calls
            await CallUpdateApiAndVerifyAsync(i);

        timeProvider.Advance(TimeSpan.FromSeconds(30));
        _ = sut.UpdateRequestCount.Should().Be(5);

        // 6th call (waits 1 minute)
        var task = sut.Member.CreateAsync([]);
        _ = sut.UpdateRequestCount.Should().Be(5);
        timeProvider.Advance(TimeSpan.FromSeconds(30));
        await task;
        _ = sut.UpdateRequestCount.Should().Be(1);

        handler.VerifyAnyRequest(Times.Exactly(6));

        async Task CallUpdateApiAndVerifyAsync(int expected)
        {
            _ = await sut.Member.CreateAsync([]);
            _ = sut.UpdateRequestCount.Should().Be(expected);
        }
    }

    /// <summary>
    /// 更新リクエスト制限の対象となるAPIは、エラー発生時に実行回数としてカウントされない。
    /// </summary>
    [TestMethod($"API Caller > 更新リクエスト制限の対象となるAPIは、エラー発生時に実行回数としてカウントされない。"), TestCategory("API")]
    public async Task When_Api_Returns_Error_UpdateApi_DoesNot_Counts_UpdateLimit()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
            .ReturnsResponse(HttpStatusCode.NotFound, /*lang=json,strict*/ """{"errors":["test"]}""", "application/json");

        // Act
        var sut = CreateSut(handler, "token");
        var act = async () => await sut.Member.OverWriteAsync([]);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ApplicationException>();
        _ = sut.UpdateRequestCount.Should().Be(0);
    }

    /// <summary>
    /// <see cref="KaonaviClient.Dispose"/>を呼び出した後のAPI呼び出しは、<see cref="ObjectDisposedException"/>の例外をスローする。。
    /// </summary>
    [TestMethod($"API Caller > ${nameof(KaonaviClient.Dispose)}()後にAPIを呼び出そうとした場合、{nameof(ObjectDisposedException)}の例外をスローする。"), TestCategory("API")]
    public async Task When_Disposed_Api_Throws_ObjectDisposedException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");
        var timeProvider = new FakeTimeProvider();

        // Act
        var sut = CreateSut(handler, "token", timeProvider);
        _ = await sut.Member.OverWriteAsync([]);
        sut.Dispose();
        var act = async () => await sut.Member.OverWriteAsync([]);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ObjectDisposedException>();
        handler.VerifyAnyRequest(Times.Once());
    }
    #endregion API Common Path

    /// <summary>
    /// <see cref="KaonaviClient.AuthenticateAsync"/>は、"/token"にBase64文字列のPOSTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviClient.AuthenticateAsync)} > POST /token をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("アクセストークン")]
    public async Task AuthenticateAsync_Calls_PostApi()
    {
        // Arrange
        string key = FixtureFactory.Create<string>();
        string secret = FixtureFactory.Create<string>();
        string tokenString = FixtureFactory.Create<string>();
        var response = new Token(FixtureFactory.Create<string>(), "Bearer", 3600);

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/token")
            .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

        // Act
        var sut = CreateSut(handler, key: key, secret: secret);
        var token = await sut.AuthenticateAsync();

        // Assert
        _ = token.Should().Be(response);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Should().SendTo(HttpMethod.Post, "/token");

            // Header
            _ = (req.Headers.Authorization?.Scheme.Should().Be("Basic"));
            byte[] byteArray = Encoding.UTF8.GetBytes($"{key}:{secret}");
            string base64String = Convert.ToBase64String(byteArray);
            _ = (req.Headers.Authorization?.Parameter.Should().Be(base64String));

            // Body
            _ = req.Content.Should().BeAssignableTo<FormUrlEncodedContent>();
            string body = await req.Content!.ReadAsStringAsync();
            _ = body.Should().Be("grant_type=client_credentials");

            return true;
        }, Times.Once());
    }
}
