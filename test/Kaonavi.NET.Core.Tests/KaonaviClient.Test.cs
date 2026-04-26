using System.Text;
using System.Text.Json;
using Kaonavi.Net.Entities;
using Microsoft.Extensions.Time.Testing;
using TUnit.Mocks.Http;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests;

/// <summary><see cref="KaonaviClient"/>の単体テスト</summary>
public sealed partial class KaonaviClientTest
{
    /// <summary>テスト用の<see cref="HttpClient.BaseAddress"/></summary>
    private const string BaseUriString = "https://example.com/";
    /// <summary>テスト用の<see cref="HttpClient.BaseAddress"/></summary>
    private static readonly Uri _baseUri = new("https://example.com/");

    /// <summary><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></summary>
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
    private static KaonaviClient CreateSut(MockHttpClient client, string? accessToken = null, TimeProvider? timeProvider = null, string key = "Key", string secret = "Secret")
        => new(client, key, secret, timeProvider ?? TimeProvider.System)
        {
            AccessToken = accessToken
        };

    #region Constructor
    /// <summary>
    /// <paramref name="consumerKey"/>または<paramref name="consumerSecret"/>が<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    /// <param name="consumerKey"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerKey']"/></param>
    /// <param name="consumerSecret"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></param>
    /// <param name="paramName">例外の原因となったパラメータ名</param>
    [Test($"{nameof(KaonaviClient)}(constructor) > {nameof(ArgumentNullException)}をスローする。")]
    [Category("Constructor")]
    [Arguments(null, "foo", "consumerKey", DisplayName = $"{nameof(KaonaviClient)}({nameof(HttpClient)}, null, <non-null>) > {nameof(ArgumentNullException)}(consumerKey)をスローする。")]
    [Arguments("foo", null, "consumerSecret", DisplayName = $"{nameof(KaonaviClient)}({nameof(HttpClient)}, <non-null>, null) > {nameof(ArgumentNullException)}(consumerSecret)をスローする。")]
    public async Task WhenParamIsNull_Constructor_Throws_ArgumentNullException(string? consumerKey, string? consumerSecret, string paramName)
        => await Assert.That(() => _ = new KaonaviClient(new(), consumerKey!, consumerSecret!))
            .Throws<ArgumentNullException>().WithParameterName(paramName);

    /// <summary>
    /// HttpClientが<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    [Test($"{nameof(KaonaviClient)}(null, <non-null>, <non-null>) > ArgumentNullException(client)をスローする。")]
    [Category("Constructor")]
    public async Task WhenClientIsNull_Constructor_Throws_ArgumentNullException()
        => await Assert.That(() => _ = new KaonaviClient(null!, "foo", "bar"))
            .Throws<ArgumentNullException>().WithParameterName("client");

    /// <summary>
    /// TimeProviderが<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    [Test($"{nameof(KaonaviClient)}({nameof(HttpClient)}, <non-null>, <non-null>, null) > ArgumentNullException(timeProvider)をスローする。")]
    [Category("Constructor")]
    public async Task WhenTimeProviderIsNull_Constructor_Throws_ArgumentNullException()
        => await Assert.That(() => _ = new KaonaviClient(new(), "foo", "bar", null!))
            .Throws<ArgumentNullException>().WithParameterName("timeProvider");

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>のとき、既定値をセットする。
    /// </summary>
    [Test($"{nameof(KaonaviClient)}(constructor) > {nameof(HttpClient.BaseAddress)}がnullのとき、既定値をセットする。")]
    [Category("Constructor")]
    public async Task Constructor_Sets_BaseAddress_WhenIsNull()
    {
        // Arrange
        var client = new HttpClient();
        await Assert.That(client.BaseAddress).IsNull();

        // Act
        _ = new KaonaviClient(client, "foo", "bar");

        // Assert
        await Assert.That(client.BaseAddress).IsNotNull();
    }

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>でないときは、既定値をセットしない。
    /// </summary>
    [Test($"{nameof(KaonaviClient)}(constructor) > {nameof(HttpClient.BaseAddress)}がnullでないときは、既定値をセットしない。")]
    [Category("Constructor")]
    public async Task Constructor_DoesNotSet_BaseAddress_WhenNotNull()
    {
        // Arrange
        var client = new HttpClient
        {
            BaseAddress = _baseUri
        };

        // Act
        _ = new KaonaviClient(client, "foo", "bar");

        // Assert
        await Assert.That(client.BaseAddress).IsEqualTo(_baseUri);
    }
    #endregion Constructor

    #region Properties
    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を返す。
    /// </summary>
    [Test($"{nameof(KaonaviClient.AccessToken)}(get) > Kaonavi-Tokenヘッダーの値を返す。")]
    [Category("Properties")]
    public async Task AccessToken_Returns_Header_KaonaviToken_Value()
    {
        // Arrange
        var client = new HttpClient();
        const string headerValue = "token";
        client.DefaultRequestHeaders.Add("Kaonavi-Token", headerValue);

        // Act
        var sut = new KaonaviClient(client, "foo", "bar");

        // Assert
        await Assert.That(sut.AccessToken).IsEqualTo(headerValue);
    }

    /// <summary>
    /// Kaonavi-Tokenヘッダーがないとき、<see cref="KaonaviClient.AccessToken"/>は、<see langword="null"/>を返す。
    /// </summary>
    [Test($"{nameof(KaonaviClient.AccessToken)}(get) > Kaonavi-Tokenヘッダーがないとき、nullを返す。")]
    [Category("Properties")]
    public async Task When_Header_KaonaviToken_IsNull_AccessToken_Returns_Null()
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var sut = new KaonaviClient(client, "foo", "bar");

        // Assert
        await Assert.That(client.DefaultRequestHeaders.TryGetValues("Kaonavi-Token", out _)).IsFalse();
        await Assert.That(sut.AccessToken).IsNull();
    }

    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を設定する。
    /// </summary>
    [Test($"{nameof(KaonaviClient.AccessToken)}(set) > Kaonavi-Tokenヘッダーの値を設定する。")]
    [Category("Properties")]
    public async Task AccessToken_Sets_Header_KaonaviToken()
    {
        // Arrange
        var client = new HttpClient();
        const string headerValue = "token";

        // Act
        _ = new KaonaviClient(client, "foo", "bar")
        {
            AccessToken = headerValue
        };

        // Assert
        await Assert.That(client.DefaultRequestHeaders.TryGetValues("Kaonavi-Token", out var values)).IsTrue();
        await Assert.That(values).IsNotNull().And.IsEquivalentTo([headerValue]);
    }

    /// <summary>
    /// <see cref="KaonaviClient.UseDryRun"/>は、HttpClientのDry-Runヘッダーが"1"かどうかを返す。
    /// </summary>
    /// <param name="headerValue">Dry-Runヘッダーに設定する値</param>
    /// <param name="expected"><see cref="KaonaviClient.UseDryRun"/></param>
    [Test($"{nameof(KaonaviClient.UseDryRun)}(get) > Dry-Run: 1 かどうかを返す。")]
    [Category("Properties")]
    [Arguments(null, false, DisplayName = $"{nameof(KaonaviClient.UseDryRun)} > Headerに Dry-Runの設定がない場合、 falseを返す。")]
    [Arguments("0", false, DisplayName = $"{nameof(KaonaviClient.UseDryRun)} > Headerに Dry-Run: 0 の設定がある場合、 falseを返す。")]
    [Arguments("1", true, DisplayName = $"{nameof(KaonaviClient.UseDryRun)} > Headerに Dry-Run: 1 の設定がある場合、 trueを返す。")]
    [Arguments("foo", false, DisplayName = $"{nameof(KaonaviClient.UseDryRun)} > Headerに Dry-Run: foo の設定がある場合、 falseを返す。")]
    public async Task UseDryRun_Returns_Header_DryRun_Is1(string? headerValue, bool expected)
    {
        // Arrange
        var client = new HttpClient();
        if (headerValue is not null)
            client.DefaultRequestHeaders.Add("Dry-Run", headerValue);

        // Act
        var sut = new KaonaviClient(client, "foo", "bar");

        // Assert
        await Assert.That(sut.UseDryRun).IsEqualTo(expected);
    }

    /// <summary>
    /// <see cref="KaonaviClient.UseDryRun"/>は、HttpClientのDry-Runヘッダー値を追加/削除する。
    /// </summary>
    [Test($"{nameof(KaonaviClient.UseDryRun)}(set) > Dry-Runヘッダーを追加/削除する。")]
    [Category("Properties")]
    public async Task UseDryRun_Sets_Header_DryRun()
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
        await Assert.That(client.DefaultRequestHeaders.TryGetValues("Dry-Run", out var values)).IsTrue();
        await Assert.That(values).IsNotNull().And.IsEquivalentTo(["1"]);
        #endregion UseDryRun = true

        #region UseDryRun = false
        // Act
        sut.UseDryRun = false;

        // Assert
        await Assert.That(client.DefaultRequestHeaders.TryGetValues("Dry-Run", out _)).IsFalse();
        #endregion UseDryRun = false
    }
    #endregion Properties

    #region API Common Path
    /// <summary>
    /// サーバー側がエラーを返したとき、<see cref="ApplicationException"/>の例外をスローする。
    /// </summary>
    /// <param name="statusCode">HTTPステータスコード</param>
    /// <param name="mediaType">MediaType</param>
    /// <param name="responseBody">エラー時のレスポンス本文</param>
    /// <param name="expectedMessage">エラーメッセージ</param>
    [Test($"API Caller > サーバー側がエラーを返したとき、{nameof(ApplicationException)}の例外をスローする。")]
    [Category("API")]
    [Arguments(HttpStatusCode.InternalServerError, null, "Internal Server Error", "Internal Server Error", DisplayName = "サーバーが500 Internal Server Errorを返したとき、ApplicationExceptionをスローする。")]
    [Arguments(HttpStatusCode.BadRequest, "application/json", /* lang=json,strict */"""{"errors":["test"]}""", "test", DisplayName = "サーバーが400 Bad Requestでエラー内容をJSONで返したとき、ApplicationExceptionをスローする。")]
    public async Task When_Server_Returns_Error_Api_Throws_ApplicationException(HttpStatusCode statusCode, string? mediaType, string responseBody, string expectedMessage)
    {
        // Arrange
        using var client = Mock.HttpClient(BaseUriString);
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(responseBody, Encoding.UTF8)
            {
                Headers =
                {
                    ContentType = mediaType is not null ? new(mediaType) : null
                }
            }
        };
        client.Handler.OnAnyRequest().RespondWith(response);

        // Act - Assert
        var sut = CreateSut(client, "token");
        await Assert.That(async () => await sut.Layout.ReadMemberLayoutAsync())
            .Throws<ApplicationException>().WithMessage(expectedMessage);
    }

    /// <summary>
    /// <see cref="KaonaviClient.AccessToken"/>が<see langword="null"/>のとき、<see cref="KaonaviClient.AuthenticateAsync(CancellationToken)"/>を呼び出す。
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.AuthenticateAsync" path="/param[@name='cancellationToken']"/></param>
    [Test($"API Caller > {nameof(KaonaviClient.AuthenticateAsync)}を呼び出す。")]
    [Category("API")]
    public async Task When_AccessToken_IsNull_ApiCaller_Calls_AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        // Arrange
        using var client = Mock.HttpClient(BaseUriString);
        client.Handler.OnAnyRequest().RespondWithString("Error", HttpStatusCode.InternalServerError);

        // Act - Assert
        var sut = CreateSut(client);
        await Assert.That(async () => await sut.Layout.ReadMemberLayoutAsync(cancellationToken: cancellationToken))
            .Throws<ApplicationException>().WithMessage("Error");
        client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/token"), Times.Once);
    }

    /// <summary>
    /// 更新リクエスト制限の対象となるAPIは、6回目の呼び出し前に1分間待機する。
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.CreateAsync" path="/param[@name='cancellationToken']"/></param>
    [Test("API Caller > 更新リクエスト制限の対象となるAPIは、6回目の呼び出し前に1分間待機する。")]
    [Category("API")]
    public async Task UpdateApi_Waits_UpdateLimit(CancellationToken cancellationToken = default)
    {
        // Arrange
        using var client = Mock.HttpClient(BaseUriString);
        client.Handler.OnPost("/members").RespondWithJson(TaskJson);
        var timeProvider = new FakeTimeProvider();

        // Act - Assert
        var sut = CreateSut(client, "token", timeProvider);
        await Assert.That(sut.UpdateRequestCount).IsEqualTo(0);

        for (int i = 1; i <= 5; i++) // 1-5th calls
            await CallUpdateApiAndVerifyAsync(i);

        timeProvider.Advance(TimeSpan.FromSeconds(30));
        await Assert.That(sut.UpdateRequestCount).IsEqualTo(5);

        // 6th call (does not call API until 1 min)
        var task = sut.Member.CreateAsync([], cancellationToken);
        await Assert.That(sut.UpdateRequestCount).IsEqualTo(5);
        client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/members"), Times.Exactly(5));

        timeProvider.Advance(TimeSpan.FromSeconds(30));
        await task;
        await Assert.That(sut.UpdateRequestCount).IsEqualTo(1);
        client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/members"), Times.Exactly(6));

        async ValueTask CallUpdateApiAndVerifyAsync(int expected)
        {
            _ = await sut.Member.CreateAsync([], cancellationToken);
            await Assert.That(sut.UpdateRequestCount).IsEqualTo(expected);
            client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/members"), Times.Exactly(expected));
        }
    }

    /// <summary>
    /// 更新リクエスト制限の対象となるAPIは、エラー発生時に実行回数としてカウントされない。
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.OverWriteAsync" path="/param[@name='cancellationToken']"/></param>
    [Test("API Caller > 更新リクエスト制限の対象となるAPIは、エラー発生時に実行回数としてカウントされない。")]
    [Category("API")]
    public async Task When_Api_Returns_Error_UpdateApi_DoesNot_Counts_UpdateLimit(CancellationToken cancellationToken = default)
    {
        // Arrange
        using var client = Mock.HttpClient(BaseUriString);
        client.Handler.OnPut("/members/overwrite").RespondWithJson(/* lang=json,strict */"""{"errors":["test"]}""", HttpStatusCode.NotFound);

        // Act - Assert
        var sut = CreateSut(client, "token");
        await Assert.That(async () => await sut.Member.OverWriteAsync([], cancellationToken))
            .Throws<ApplicationException>().WithMessageContaining("test");
        await Assert.That(sut.UpdateRequestCount).IsEqualTo(0);
        client.Handler.Verify(r => r.Method(HttpMethod.Put).Path("/members/overwrite"), Times.Once);
    }

    /// <summary>
    /// <see cref="KaonaviClient.Dispose"/>を呼び出した後のAPI呼び出しは、<see cref="ObjectDisposedException"/>の例外をスローする。
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IMember.OverWriteAsync" path="/param[@name='cancellationToken']"/></param>
    [Test($"API Caller > ${nameof(KaonaviClient.Dispose)}()後にAPIを呼び出そうとした場合、{nameof(ObjectDisposedException)}の例外をスローする。")]
    [Category("API")]
    public async Task When_Disposed_Api_Throws_ObjectDisposedException(CancellationToken cancellationToken = default)
    {
        // Arrange
        using var client = Mock.HttpClient(BaseUriString);
        client.Handler.OnPut("/members/overwrite").RespondWithJson(TaskJson);

        // Act
        var sut = CreateSut(client, "token");
        _ = await sut.Member.OverWriteAsync([], cancellationToken);
        sut.Dispose();

        // Assert
        await Assert.That(async () => await sut.Member.OverWriteAsync([], cancellationToken)).Throws<ObjectDisposedException>();
        client.Handler.Verify(r => r.Method(HttpMethod.Put).Path("/members/overwrite"), Times.Once);
    }
    #endregion API Common Path

    /// <summary>
    /// <see cref="KaonaviClient.AuthenticateAsync"/>は、"/token"にBase64文字列のPOSTリクエストを行う。
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.AuthenticateAsync" path="/param[@name='cancellationToken']"/></param>
    [Test($"{nameof(KaonaviClient.AuthenticateAsync)} > POST /token をコールする。")]
    [Category("API"), Category(nameof(HttpMethod.Post)), Category("アクセストークン")]
    public async Task AuthenticateAsync_Calls_PostApi(CancellationToken cancellationToken = default)
    {
        // Arrange
        const string key = "test_key";
        const string secret = "test_secret";
        var response = new Token("token", "Bearer", 3600);
        string responseJson = JsonSerializer.Serialize(response, JsonContext.Default.Token);

        using var client = Mock.HttpClient(BaseUriString);
        client.Handler.OnPost("/token").RespondWithJson(responseJson);

        // Act
        var sut = CreateSut(client, key: key, secret: secret);
        var token = await sut.AuthenticateAsync(cancellationToken);

        // Assert
        await Assert.That(token).IsEqualTo(response);
        client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/token"), Times.Once);
        var req = client.Handler.Requests[0];
        await Assert.That(req.Headers.TryGetValue("Authorization", out var authValues)).IsTrue();
        string? authHeader = authValues?.FirstOrDefault();
        await Assert.That(authHeader).IsNotNull();
        await Assert.That(authHeader).StartsWith("Basic ");
        string expectedParam = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}"));
        await Assert.That(authHeader).IsEqualTo($"Basic {expectedParam}");
        await Assert.That(req.Body).IsEqualTo("grant_type=client_credentials");
    }
}
