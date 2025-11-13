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
    [TestMethod(DisplayName = $"{nameof(KaonaviClient)}(constructor) > {nameof(ArgumentNullException)}をスローする。"), TestCategory("Constructor")]
    [DataRow(null, "foo", "consumerKey", DisplayName = $"{nameof(KaonaviClient)}({nameof(HttpClient)}, null, \"foo\") > {nameof(ArgumentNullException)}(consumerKey)をスローする。")]
    [DataRow("foo", null, "consumerSecret", DisplayName = $"{nameof(KaonaviClient)}({nameof(HttpClient)}, \"foo\", null) > {nameof(ArgumentNullException)}(consumerSecret)をスローする。")]
    public void WhenParamIsNull_Constructor_Throws_ArgumentNullException(string? consumerKey, string? consumerSecret, string paramName)
        => Constructor(new(), consumerKey, consumerSecret).ShouldThrow<ArgumentNullException>().ParamName.ShouldBe(paramName);

    /// <summary>
    /// HttpClientが<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient)}(null, \"foo\", \"bar\") > ArgumentNullException(client)をスローする。"), TestCategory("Constructor")]
    public void WhenClientIsNull_Constructor_Throws_ArgumentNullException()
        => Constructor(null, "foo", "bar").ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("client");

    /// <summary>
    /// TimeProviderが<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient)}({nameof(HttpClient)}, \"foo\", \"bar\", null) > ArgumentNullException(timeProvider)をスローする。"), TestCategory("Constructor")]
    public void WhenTimeProviderIsNull_Constructor_Throws_ArgumentNullException()
        => ((Action)(() => _ = new KaonaviClient(new(), "foo", "bar", null!)))
            .ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("timeProvider");

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>のとき、既定値をセットする。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient)}(constructor) > {nameof(HttpClient.BaseAddress)}がnullのとき、既定値をセットする。"), TestCategory("Constructor")]
    public void Constructor_Sets_BaseAddress_WhenIsNull()
    {
        // Arrange
        var client = new HttpClient();
        client.BaseAddress.ShouldBeNull();

        // Act
        _ = new KaonaviClient(client, "foo", "bar");

        // Assert
        client.BaseAddress.ShouldNotBeNull();
    }

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>でないときは、既定値をセットしない。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient)}(constructor) > {nameof(HttpClient.BaseAddress)}がnullでないときは、既定値をセットしない。"), TestCategory("Constructor")]
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
        client.BaseAddress.ShouldBe(_baseUri);
    }
    #endregion Constructor

    #region Properties
    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を返す。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient.AccessToken)}(get) > Kaonavi-Tokenヘッダーの値を返す。"), TestCategory("Properties")]
    public void AccessToken_Returns_Header_KaonaviToken_Value()
    {
        // Arrange
        var client = new HttpClient();
        string headerValue = FixtureFactory.Create<string>();
        client.DefaultRequestHeaders.Add("Kaonavi-Token", headerValue);

        // Act
        var sut = new KaonaviClient(client, "foo", "bar");

        // Assert
        sut.AccessToken.ShouldBe(headerValue);
    }

    /// <summary>
    /// Kaonavi-Tokenヘッダーがないとき、<see cref="KaonaviClient.AccessToken"/>は、<see langword="null"/>を返す。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient.AccessToken)}(get) > Kaonavi-Tokenヘッダーがないとき、nullを返す。"), TestCategory("Properties")]
    public void When_Header_KaonaviToken_IsNull_AccessToken_Returns_Null()
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var sut = new KaonaviClient(client, "foo", "bar");

        // Assert
        client.DefaultRequestHeaders.TryGetValues("Kaonavi-Token", out _).ShouldBeFalse();
        sut.AccessToken.ShouldBeNull();
    }

    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を設定する。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient.AccessToken)}(set) > Kaonavi-Tokenヘッダーの値を設定する。"), TestCategory("Properties")]
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
        client.DefaultRequestHeaders.TryGetValues("Kaonavi-Token", out var values).ShouldBeTrue();
        values.ShouldHaveSingleItem().ShouldBe(headerValue);
    }

    /// <summary>
    /// <see cref="KaonaviClient.UseDryRun"/>は、HttpClientのDry-Runヘッダーが"1"かどうかを返す。
    /// </summary>
    /// <param name="headerValue">Dry-Runヘッダーに設定する値</param>
    /// <param name="expected"><see cref="KaonaviClient.UseDryRun"/></param>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient.UseDryRun)}(get) > Dry-Run: 1 かどうかを返す。"), TestCategory("Properties")]
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
        sut.UseDryRun.ShouldBe(expected);
    }

    /// <summary>
    /// <see cref="KaonaviClient.UseDryRun"/>は、HttpClientのDry-Runヘッダー値を追加/削除する。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient.UseDryRun)}(set) > Dry-Runヘッダーを追加/削除する。"), TestCategory("Properties")]
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
        client.DefaultRequestHeaders.TryGetValues("Dry-Run", out var values).ShouldBeTrue();
        values.ShouldHaveSingleItem().ShouldBe("1");
        #endregion UseDryRun = true

        #region UseDryRun = false
        // Act
        sut.UseDryRun = false;

        // Assert
        client.DefaultRequestHeaders.TryGetValues("Dry-Run", out _).ShouldBeFalse();
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
    [TestMethod(DisplayName = $"API Caller > ApplicationExceptionをスローする。"), TestCategory("API")]
    [DataRow(HttpStatusCode.Unauthorized, "application/json", /*lang=json,strict*/ """{"errors":["consumer_keyとconsumer_secretの組み合わせが不正です。"]}""", "consumer_keyとconsumer_secretの組み合わせが不正です。", DisplayName = $"API Caller > ApplicationExceptionをスローする。")]
    [DataRow(HttpStatusCode.TooManyRequests, "application/json", /*lang=json,strict*/ """{"errors":["1時間あたりのトークン発行可能数を超過しました。時間をおいてお試しください。"]}""", "1時間あたりのトークン発行可能数を超過しました。時間をおいてお試しください。", DisplayName = $"API Caller > ApplicationExceptionをスローする。")]
    [DataRow(HttpStatusCode.InternalServerError, "plain/text", "Error", "Error", DisplayName = $"API Caller > ApplicationExceptionをスローする。")]
    public async ValueTask ApiCaller_Throws_ApplicationException(HttpStatusCode statusCode, string mediaType, string responseBody, string message)
    {
        // Arrange
        var mockedApi = new Mock<HttpMessageHandler>();
        _ = mockedApi.SetupAnyRequest()
            .ReturnsResponse(statusCode, responseBody, mediaType);

        // Act
        var sut = CreateSut(mockedApi);
        var act = async () => await sut.AuthenticateAsync();

        // Assert
        (await act.ShouldThrowAsync<ApplicationException>())
            .ShouldSatisfyAllConditions(
                ex => ex.Message.ShouldBe(message),
                static ex => ex.InnerException.ShouldBeAssignableTo<HttpRequestException>()
            );
    }

    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>が<see langword="null"/>のとき、<see cref="KaonaviClient.AuthenticateAsync(CancellationToken)"/>を呼び出す。
    /// </summary>
    [TestMethod(DisplayName = $"API Caller > {nameof(KaonaviClient.AuthenticateAsync)}を呼び出す。"), TestCategory("API")]
    public async ValueTask When_AccessToken_IsNull_ApiCaller_Calls_AuthenticateAsync()
    {
        // Arrange
        string key = FixtureFactory.Create<string>();
        string secret = FixtureFactory.Create<string>();

        var mockedApi = new Mock<HttpMessageHandler>();
        _ = mockedApi.SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.InternalServerError, "Error", "text/plain");

        // Act
        var sut = CreateSut(mockedApi, key: key, secret: secret);
        var act = async () => await sut.Layout.ReadMemberLayoutAsync();

        // Assert
        _ = await act.ShouldThrowAsync<ApplicationException>();

        mockedApi.ShouldBeCalledOnce(
            static req => req.Method.ShouldBe(HttpMethod.Post),
            static req => req.RequestUri?.PathAndQuery.ShouldBe("/token")
        );
    }

    /// <summary>
    /// 更新リクエスト制限の対象となるAPIは、6回目の呼び出し前に1分間待機する。
    /// </summary>
    [TestMethod(DisplayName = $"API Caller > 更新リクエスト制限の対象となるAPIは、6回目の呼び出し前に1分間待機する。"), TestCategory("API")]
    public async ValueTask UpdateApi_Waits_UpdateLimit()
    {
        // Arrange
        string token = FixtureFactory.Create<string>();
        var mockedApi = new Mock<HttpMessageHandler>();
        _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");
        var timeProvider = new FakeTimeProvider();

        // Act - Assert
        var sut = CreateSut(mockedApi, token, timeProvider);
        sut.UpdateRequestCount.ShouldBe(0);

        for (int i = 1; i <= 5; i++) // 1-5th calls
            await CallUpdateApiAndVerifyAsync(i);

        timeProvider.Advance(TimeSpan.FromSeconds(30));
        sut.UpdateRequestCount.ShouldBe(5);

        // 6th call (does not call API until 1 min)
        var task = sut.Member.CreateAsync([]);
        sut.UpdateRequestCount.ShouldBe(5);
        mockedApi.ShouldBeCalled(Times.Exactly(5));

        timeProvider.Advance(TimeSpan.FromSeconds(30));
        await task;
        sut.UpdateRequestCount.ShouldBe(1);
        mockedApi.ShouldBeCalled(Times.Exactly(6));

        async ValueTask CallUpdateApiAndVerifyAsync(int expected)
        {
            _ = await sut.Member.CreateAsync([]);
            sut.UpdateRequestCount.ShouldBe(expected);
            mockedApi.ShouldBeCalled(Times.Exactly(expected), req => req.Headers.GetValues("Kaonavi-Token").ShouldHaveSingleItem().ShouldBe(token));
        }
    }

    /// <summary>
    /// 更新リクエスト制限の対象となるAPIは、エラー発生時に実行回数としてカウントされない。
    /// </summary>
    [TestMethod(DisplayName = $"API Caller > 更新リクエスト制限の対象となるAPIは、エラー発生時に実行回数としてカウントされない。"), TestCategory("API")]
    public async ValueTask When_Api_Returns_Error_UpdateApi_DoesNot_Counts_UpdateLimit()
    {
        // Arrange
        var mockedApi = new Mock<HttpMessageHandler>();
        _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
            .ReturnsResponse(HttpStatusCode.NotFound, /*lang=json,strict*/ """{"errors":["test"]}""", "application/json");

        // Act
        var sut = CreateSut(mockedApi, "token");
        var act = async () => await sut.Member.OverWriteAsync([]);

        // Assert
        await act.ShouldThrowAsync<ApplicationException>();
        sut.UpdateRequestCount.ShouldBe(0);
        mockedApi.ShouldBeCalledOnce();
    }

    /// <summary>
    /// <see cref="KaonaviClient.Dispose"/>を呼び出した後のAPI呼び出しは、<see cref="ObjectDisposedException"/>の例外をスローする。
    /// </summary>
    [TestMethod(DisplayName = $"API Caller > ${nameof(KaonaviClient.Dispose)}()後にAPIを呼び出そうとした場合、{nameof(ObjectDisposedException)}の例外をスローする。"), TestCategory("API")]
    public async ValueTask When_Disposed_Api_Throws_ObjectDisposedException()
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
        await act.ShouldThrowAsync<ObjectDisposedException>();
        handler.ShouldBeCalledOnce();
    }
    #endregion API Common Path

    /// <summary>
    /// <see cref="KaonaviClient.AuthenticateAsync"/>は、"/token"にBase64文字列のPOSTリクエストを行う。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(KaonaviClient.AuthenticateAsync)} > POST /token をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("アクセストークン")]
    public async ValueTask AuthenticateAsync_Calls_PostApi()
    {
        // Arrange
        string key = FixtureFactory.Create<string>();
        string secret = FixtureFactory.Create<string>();
        var response = new Token(FixtureFactory.Create<string>(), "Bearer", 3600);

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/token")
            .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

        // Act
        var sut = CreateSut(handler, key: key, secret: secret);
        var token = await sut.AuthenticateAsync();

        // Assert
        token.ShouldBe(response);
        handler.ShouldBeCalledOnce(
            static req => req.Method.ShouldBe(HttpMethod.Post),
            static req => req.RequestUri?.PathAndQuery.ShouldBe("/token"),
            static req => req.Headers.Authorization?.Scheme.ShouldBe("Basic"),
            req => req.Headers.Authorization?.Parameter.ShouldBe(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}"))),
            static req => req.Content.ShouldBeAssignableTo<FormUrlEncodedContent>(),
            static async req => (await req.Content!.ReadAsStringAsync()).ShouldBe("grant_type=client_credentials")
        );
    }
}
