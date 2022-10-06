using System.Text;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Services;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests.Services;

/// <summary>
/// <see cref="KaonaviV2Service"/>の単体テスト
/// </summary>
public class KaonaviV2ServiceTest
{
    /// <summary>テスト用の<see cref="HttpClient.BaseAddress"/></summary>
    private static readonly Uri _baseUri = new("https://example.com/");

    /// <summary>タスク結果JSON</summary>
    private const string TaskJson = "{\"task_id\":1}";

    /// <summary>ランダムな文字列を生成します。</summary>
    private static string GenerateRandomString() => Guid.NewGuid().ToString();

    #region Constractor
    /// <summary>
    /// コンストラクターを呼び出す<see cref="Action"/>を生成します。
    /// </summary>
    /// <param name="client"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='client']"/></param>
    /// <param name="consumerKey"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='consumerKey']"/></param>
    /// <param name="consumerSecret"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></param>
    private static Action Constractor(HttpClient? client, string? consumerKey, string? consumerSecret)
        => () => _ = new KaonaviV2Service(client!, consumerKey!, consumerSecret!);

    /// <summary>
    /// <paramref name="consumerKey"/>または<paramref name="consumerSecret"/>が<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    /// <param name="consumerKey"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='consumerKey']"/></param>
    /// <param name="consumerSecret"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></param>
    [Theory(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(Constractor)} > ArgumentNullExceptionをスローする。")]
    [InlineData(null, "foo")]
    [InlineData("foo", null)]
    public void Constractor_Throws_ArgumentNullException_WhenKeyIsNull(string? consumerKey, string? consumerSecret)
        => Constractor(new(), consumerKey, consumerSecret).Should().ThrowExactly<ArgumentNullException>();

    /// <summary>
    /// HttpClientが<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(Constractor)} > ArgumentNullExceptionをスローする。")]
    public void Constractor_Throws_ArgumentNullException_WhenClientIsNull()
        => Constractor(null, "foo", "bar").Should().ThrowExactly<ArgumentNullException>();

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>のとき、既定値をセットする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(Constractor)} > {nameof(HttpClient.BaseAddress)}がnullのとき、既定値をセットする。")]
    public void Constractor_Sets_BaseAddress_WhenIsNull()
    {
        // Arrange
        var client = new HttpClient();
        _ = client.BaseAddress.Should().BeNull();

        // Act
        _ = new KaonaviV2Service(client, "foo", "bar");

        // Assert
        _ = client.BaseAddress.Should().NotBeNull();
    }

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>でないときは、既定値をセットしない。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(Constractor)} > {nameof(HttpClient.BaseAddress)}がnullでないときは、既定値をセットしない。")]
    public void Constractor_DoesNotSet_BaseAddress_WhenNotNull()
    {
        // Arrange
        var client = new HttpClient
        {
            BaseAddress = _baseUri
        };

        // Act
        _ = new KaonaviV2Service(client, "foo", "bar");

        // Assert
        _ = client.BaseAddress.Should().Be(_baseUri);
    }
    #endregion Constractor

    #region Properties
    /// <summary>
    /// <see cref="KaonaviV2Service.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を返す。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.AccessToken)}(get) > Kaonavi-Tokenヘッダーの値を返す。")]
    public void AccessToken_Returns_ClientHeader()
    {
        // Arrange
        var client = new HttpClient();
        string headerValue = GenerateRandomString();
        client.DefaultRequestHeaders.Add("Kaonavi-Token", headerValue);

        // Act
        var sut = new KaonaviV2Service(client, "foo", "bar");

        // Assert
        _ = sut.AccessToken.Should().Be(headerValue);
    }

    /// <summary>
    /// Kaonavi-Tokenヘッダーがないとき、<see cref="KaonaviV2Service.AccessToken"/>は、<see langword="null"/>を返す。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.AccessToken)}(get) > Kaonavi-Tokenヘッダーがないとき、nullを返す。")]
    public void AccessToken_Returns_Null()
    {
        // Arrange - Act
        var sut = new KaonaviV2Service(new(), "foo", "bar");

        // Assert
        _ = sut.AccessToken.Should().BeNull();
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を設定する。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.AccessToken)}(set) > Kaonavi-Tokenヘッダーの値を設定する。")]
    public void AccessToken_Sets_ClientHeader()
    {
        // Arrange
        var client = new HttpClient();
        string headerValue = GenerateRandomString();

        // Act
        _ = new KaonaviV2Service(client, "foo", "bar")
        {
            AccessToken = headerValue
        };

        // Assert
        _ = client.DefaultRequestHeaders.TryGetValues("Kaonavi-Token", out var values).Should().BeTrue();
        _ = values.Should().HaveCount(1).And.Contain(headerValue);
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.UseDryRun"/>は、HttpClientのDry-Runヘッダーが"1"かどうかを返す。
    /// </summary>
    /// <param name="headerValue">Dry-Runヘッダーに設定する値</param>
    /// <param name="expected"><see cref="KaonaviV2Service.UseDryRun"/></param>
    [Theory(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.UseDryRun)}(get) > Dry-Run: 1 かどうかを返す。")]
    [InlineData(null, false)]
    [InlineData("0", false)]
    [InlineData("1", true)]
    [InlineData("foo", false)]
    public void UseDryRun_Returns_ClientHeader(string? headerValue, bool expected)
    {
        // Arrange
        var client = new HttpClient();
        if (headerValue is not null)
            client.DefaultRequestHeaders.Add("Dry-Run", headerValue);

        // Act
        var sut = new KaonaviV2Service(client, "foo", "bar");

        // Assert
        _ = sut.UseDryRun.Should().Be(expected);
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.UseDryRun"/>は、HttpClientのDry-Runヘッダー値を追加/削除する。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.UseDryRun)}(set) > Dry-Runヘッダーを追加/削除する。")]
    public void UseDryRun_Sets_ClientHeader()
    {
        // Arrange
        var client = new HttpClient();

        #region UseDryRun = true
        // Act
        var sut = new KaonaviV2Service(client, "foo", "bar")
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

    /// <summary>
    /// テスト対象(System Under Test)となる<see cref="KaonaviV2Service"/>のインスタンスを生成します。
    /// </summary>
    /// <param name="handler">HttpClientをモックするためのHandlerオブジェクト</param>
    /// <param name="key">Consumer Key</param>
    /// <param name="secret">Consumer Secret</param>
    /// <param name="accessToken">アクセストークン</param>
    private static KaonaviV2Service CreateSut(Mock<HttpMessageHandler> handler, string key = "Key", string secret = "Secret", string? accessToken = null)
    {
        var client = handler.CreateClient();
        client.BaseAddress = _baseUri;
        return new(client, key, secret)
        {
            AccessToken = accessToken
        };
    }

    #region API Common Path
    /// <summary>
    /// サーバー側がエラーを返したとき、<see cref="ApplicationException"/>の例外をスローする。
    /// </summary>
    /// <param name="statusCode">HTTPステータスコード</param>
    /// <param name="contentFormat">エラー時のレスポンスBodyサンプル</param>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="mediaType">MediaType</param>
    [Theory(DisplayName = $"{nameof(KaonaviV2Service)} > API Caller > ApplicationExceptionをスローする。")]
    [InlineData(401, "{{\"errors\":[\"{0}\"]}}", "consumer_keyとconsumer_secretの組み合わせが不正です。", "application/json")]
    [InlineData(429, "{{\"errors\":[\"{0}\"]}}", "1時間あたりのトークン発行可能数を超過しました。時間をおいてお試しください。", "application/json")]
    [InlineData(500, "{0}", "Error", "plain/text")]
    public async Task ApiCaller_Throws_ApplicationException(int statusCode, string contentFormat, string message, string? mediaType)
    {
        // Arrange
        string key = GenerateRandomString();
        string secret = GenerateRandomString();
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/token")
            .ReturnsResponse((HttpStatusCode)statusCode, string.Format(contentFormat, message), mediaType);

        // Act
        var sut = CreateSut(handler, key, secret);
        Func<Task> act = async () => await sut.AuthenticateAsync().ConfigureAwait(false);

        _ = (await act.Should().ThrowExactlyAsync<ApplicationException>().ConfigureAwait(false))
            .WithMessage(message)
            .WithInnerExceptionExactly<HttpRequestException>();
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AccessToken"/>が<see langword="null"/>のとき、<see cref="KaonaviV2Service.AuthenticateAsync(CancellationToken)"/>を呼び出す。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > API Caller > {nameof(KaonaviV2Service.AuthenticateAsync)}を呼び出す。")]
    public async Task ApiCaller_Calls_AuthenticateAsync_When_AccessToken_IsNull()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string key = GenerateRandomString();
        string secret = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/token")
            .ReturnsResponse(HttpStatusCode.InternalServerError, "Error", "text/plain");

        // Act
        var sut = CreateSut(handler, key, secret);
        Func<Task> act = async () => await sut.FetchMemberLayoutAsync().ConfigureAwait(false);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ApplicationException>().ConfigureAwait(false);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Post);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/token"));

            // Header
            _ = (req.Headers.Authorization?.Scheme.Should().Be("Basic"));
            byte[] byteArray = Encoding.UTF8.GetBytes($"{key}:{secret}");
            string base64String = Convert.ToBase64String(byteArray);
            _ = (req.Headers.Authorization?.Parameter.Should().Be(base64String));

            // Body
            _ = req.Content.Should().BeAssignableTo<FormUrlEncodedContent>();
            string body = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = body.Should().Be("grant_type=client_credentials");

            return true;
        }, Times.Once());
        handler.VerifyRequest(res => res.RequestUri?.PathAndQuery != "/token", Times.Never());
    }
    #endregion API Common Path

    /// <summary>
    /// <see cref="KaonaviV2Service.AuthenticateAsync"/>は、"/token"にBase64文字列のPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.AuthenticateAsync)} > POST /token をコールする。")]
    public async Task AuthenticateAsync_Calls_PostApi()
    {
        // Arrange
        const string responseJson = "{"
        + "\"access_token\": \"25396f58-10f8-c228-7f0f-818b1d666b2e\","
        + "\"token_type\": \"Bearer\","
        + "\"expires_in\": 3600"
        + "}";
        string key = GenerateRandomString();
        string secret = GenerateRandomString();
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/token")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, key, secret);
        var token = await sut.AuthenticateAsync().ConfigureAwait(false);

        // Assert
        _ = token.Should().NotBeNull();

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Post);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/token"));

            // Header
            _ = (req.Headers.Authorization?.Scheme.Should().Be("Basic"));
            byte[] byteArray = Encoding.UTF8.GetBytes($"{key}:{secret}");
            string base64String = Convert.ToBase64String(byteArray);
            _ = (req.Headers.Authorization?.Parameter.Should().Be(base64String));

            // Body
            _ = req.Content.Should().BeAssignableTo<FormUrlEncodedContent>();
            string body = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = body.Should().Be("grant_type=client_credentials");

            return true;
        }, Times.Once());
    }

    #region タスク進捗状況 API
    /// <summary>
    /// <inheritdoc cref="KaonaviV2Service.FetchTaskProgressAsync" path="/param[@name='taskId']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.FetchTaskProgressAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchTaskProgressAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    public async Task FetchTaskProgressAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        Func<Task> act = async () => _ = await sut.FetchTaskProgressAsync(-1).ConfigureAwait(false);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithMessage("*taskId*")
            .ConfigureAwait(false);

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchTaskProgressAsync"/>は、"/tasks/{taskId}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchTaskProgressAsync)} > GET /tasks/:taskId をコールする。")]
    public async Task FetchTaskProgressAsync_Calls_GetApi()
    {
        // Arrange
        const int taskId = 1;
        const string responseJson = "{"
        + "\"id\": 1,"
        + "\"status\": \"NG\","
        + "\"messages\": [\"エラーメッセージ1\", \"エラーメッセージ2\"]"
        + "}";
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/tasks/{taskId}")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var task = await sut.FetchTaskProgressAsync(taskId).ConfigureAwait(false);

        // Assert
        _ = task.Should().NotBeNull();

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/tasks/{taskId}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }
    #endregion タスク進捗状況 API

    #region レイアウト設定 API
    /// <summary>
    /// <see cref="KaonaviV2Service.FetchMemberLayoutAsync"/>は、"/member_layouts"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchMemberLayoutAsync)} > GET /member_layouts をコールする。")]
    public async Task FetchMemberLayoutAsync_Calls_GetApi()
    {
        #region JSON
        const string responseJson = "{"
        + "\"code\": {"
        + "  \"name\": \"社員番号\","
        + "  \"required\": true,"
        + "  \"type\": \"string\","
        + "  \"max_length\": 50,"
        + "  \"enum\": []"
        + "},"
        + "\"name\": {"
        + "  \"name\": \"氏名\","
        + "  \"required\": false,"
        + "  \"type\": \"string\","
        + "  \"max_length\": 100,"
        + "  \"enum\": []"
        + "},"
        + "\"name_kana\": {"
        + "  \"name\": \"フリガナ\","
        + "  \"required\": false,"
        + "  \"type\": \"string\","
        + "  \"max_length\": 100,"
        + "  \"enum\": []"
        + "},"
        + "\"mail\": {"
        + "  \"name\": \"メールアドレス\","
        + "  \"required\": false,"
        + "  \"type\": \"string\","
        + "  \"max_length\": 100,"
        + "  \"enum\": []"
        + "},"
        + "\"entered_date\": {"
        + "  \"name\": \"入社日\","
        + "  \"required\": false,"
        + "  \"type\": \"date\","
        + "  \"max_length\": null,"
        + "  \"enum\": []"
        + "},"
        + "\"retired_date\": {"
        + "  \"name\": \"退職日\","
        + "  \"required\": false,"
        + "  \"type\": \"date\","
        + "  \"max_length\": null,"
        + "  \"enum\": []"
        + "},"
        + "\"gender\": {"
        + "  \"name\": \"性別\","
        + "  \"required\": false,"
        + "  \"type\": \"enum\","
        + "  \"max_length\": null,"
        + "  \"enum\": [\"男性\", \"女性\"]"
        + "},"
        + "\"birthday\": {"
        + "  \"name\": \"生年月日\","
        + "  \"required\": false,"
        + "  \"type\": \"date\","
        + "  \"max_length\": null,"
        + "  \"enum\": []"
        + "},"
        + "\"department\": {"
        + "  \"name\": \"所属\","
        + "  \"required\": false,"
        + "  \"type\": \"department\","
        + "  \"max_length\": null,"
        + "  \"enum\": []"
        + "},"
        + "\"sub_departments\": {"
        + "  \"name\": \"兼務情報\","
        + "  \"required\": false,"
        + "  \"type\": \"department[]\","
        + "  \"max_length\": null,"
        + "  \"enum\": []"
        + "},"
        + "\"custom_fields\": ["
        + "  {"
        + "    \"id\": 100,"
        + "    \"name\": \"血液型\","
        + "    \"required\": false,"
        + "    \"type\": \"enum\","
        + "    \"max_length\": null,"
        + "    \"enum\": [\"A\", \"B\", \"O\", \"AB\"]"
        + "  },"
        + "  {"
        + "    \"id\": 200,"
        + "    \"name\": \"役職\","
        + "    \"required\": false,"
        + "    \"type\": \"enum\","
        + "    \"max_length\": null,"
        + "    \"enum\": [\"部長\", \"課長\", \"マネージャー\", null]"
        + "  }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/member_layouts")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var layout = await sut.FetchMemberLayoutAsync().ConfigureAwait(false);

        // Assert
        _ = layout.Should().NotBeNull();

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/member_layouts"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchSheetLayoutsAsync"/>は、"/sheet_layouts"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchSheetLayoutsAsync)} > GET /sheet_layouts をコールする。")]
    public async Task FetchSheetLayoutsAsync_Calls_GetApi()
    {
        // Arrange
        #region JSON
        const string responseJson = "{"
        + "\"sheets\": ["
        + "  {"
        + "    \"id\": 12,"
        + "    \"name\": \"住所・連絡先\","
        + "    \"record_type\": 1,"
        + "    \"custom_fields\": ["
        + "      {"
        + "        \"id\": 1000,"
        + "        \"name\": \"住所\","
        + "        \"required\": false,"
        + "        \"type\": \"string\","
        + "        \"max_length\": 250,"
        + "        \"enum\": []"
        + "      },"
        + "      {"
        + "        \"id\": 1001,"
        + "        \"name\": \"電話番号\","
        + "        \"required\": false,"
        + "        \"type\": \"string\","
        + "        \"max_length\": 50,"
        + "        \"enum\": []"
        + "      }"
        + "    ]"
        + "  }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/sheet_layouts")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var layouts = await sut.FetchSheetLayoutsAsync().ConfigureAwait(false);

        // Assert
        _ = layouts.Should().HaveCount(1);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/sheet_layouts"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchSheetLayoutAsync"/>は、"/sheet_layouts/{sheetId}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchSheetLayoutsAsync)} > GET /sheet_layouts/:sheetId をコールする。")]
    public async Task FetchSheetLayoutAsync_Calls_GetApi()
    {
        // Arrange
        #region JSON
        const string responseJson = "{"
        + "\"id\": 12,"
        + "\"name\": \"住所・連絡先\","
        + "\"record_type\": 1,"
        + "\"custom_fields\": ["
        + "  {"
        + "    \"id\": 1000,"
        + "    \"name\": \"住所\","
        + "    \"required\": false,"
        + "    \"type\": \"string\","
        + "    \"max_length\": 250,"
        + "    \"enum\": []"
        + "  },"
        + "  {"
        + "    \"id\": 1001,"
        + "    \"name\": \"電話番号\","
        + "    \"required\": false,"
        + "    \"type\": \"string\","
        + "    \"max_length\": 50,"
        + "    \"enum\": []"
        + "  }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/sheet_layouts/12")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var layout = await sut.FetchSheetLayoutAsync(12).ConfigureAwait(false);

        // Assert
        _ = layout.Should().NotBeNull();

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/sheet_layouts/12"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").Should().Equal(tokenString);

            return true;
        }, Times.Once());
    }
    #endregion レイアウト設定 API

    #region メンバー情報 API
    /// <summary>Member APIのリクエストPayload</summary>
    private static readonly MemberData[] _memberDataPayload = new MemberData[]
    {
            new (
                Code: "A0002",
                Name: "カオナビ 太郎",
                NameKana: "カオナビ タロウ",
                Mail: "taro@example.com",
                EnteredDate: new(2005, 9, 20),
                Gender: "男性",
                Birthday: new(1984, 5, 15),
                Department: new("1000"),
                SubDepartments: Array.Empty<MemberDepartment>(),
                CustomFields: new CustomFieldValue[]
                {
                    new(100, "A")
                }
            ),
            new (
                Code: "A0001",
                Name: "カオナビ 花子",
                NameKana: "カオナビ ハナコ",
                Mail: "hanako@kaonavi.jp",
                EnteredDate: new(2013, 5, 7),
                Gender: "女性",
                Birthday: new(1986, 5, 16),
                Department: new("2000"),
                SubDepartments: new MemberDepartment[]
                {
                    new("3000"), new("4000")
                },
                CustomFields: new CustomFieldValue[]
                {
                    new(100, "O"), new(200, new[]{ "部長", "マネージャー" })
                }
            )
    };

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchMembersDataAsync"/>は、"/members"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchMembersDataAsync)} > GET /members をコールする。")]
    public async Task FetchMembersDataAsync_Calls_GetApi()
    {
        // Arrange
        #region JSON
        const string responseJson = "{"
        + "\"updated_at\": \"2020-10-01 01:23:45\","
        + "\"member_data\": ["
        + "  {"
        + "    \"code\": \"A0002\","
        + "    \"name\": \"カオナビ 太郎\","
        + "    \"name_kana\": \"カオナビ タロウ\","
        + "    \"mail\": \"taro@kaonavi.jp\","
        + "    \"entered_date\": \"2005-09-20\","
        + "    \"retired_date\": \"\","
        + "    \"gender\": \"男性\","
        + "    \"birthday\": \"1984-05-15\","
        + "    \"age\": 36,"
        + "    \"years_of_service\": \"15年5ヵ月\","
        + "    \"department\": {"
        + "      \"code\": \"1000\","
        + "      \"name\": \"取締役会\","
        + "      \"names\": ["
        + "        \"取締役会\""
        + "      ]"
        + "    },"
        + "    \"sub_departments\": [],"
        + "    \"custom_fields\": ["
        + "      {"
        + "        \"id\": 100,"
        + "        \"name\": \"血液型\","
        + "        \"values\": ["
        + "          \"A\""
        + "        ]"
        + "      }"
        + "    ]"
        + "  },"
        + "  {"
        + "    \"code\": \"A0001\","
        + "    \"name\": \"カオナビ 花子\","
        + "    \"name_kana\": \"カオナビ ハナコ\","
        + "    \"mail\": \"hanako@kaonavi.jp\","
        + "    \"entered_date\": \"2013-05-07\","
        + "    \"retired_date\": \"\","
        + "    \"gender\": \"女性\","
        + "    \"birthday\": \"1986-05-16\","
        + "    \"age\": 36,"
        + "    \"years_of_service\": \"7年9ヵ月\","
        + "    \"department\": {"
        + "      \"code\": \"2000\","
        + "      \"name\": \"営業本部 第一営業部 ITグループ\","
        + "      \"names\": ["
        + "        \"営業本部\","
        + "        \"第一営業部\","
        + "        \"ITグループ\""
        + "      ]"
        + "    },"
        + "    \"sub_departments\": ["
        + "      {"
        + "        \"code\": \"3000\","
        + "        \"name\": \"企画部\","
        + "        \"names\": ["
        + "          \"企画部\""
        + "        ]"
        + "      },"
        + "      {"
        + "        \"code\": \"4000\","
        + "        \"name\": \"管理部\","
        + "        \"names\": ["
        + "          \"管理部\""
        + "        ]"
        + "      }"
        + "    ],"
        + "    \"custom_fields\": ["
        + "      {"
        + "        \"id\": 100,"
        + "        \"name\": \"血液型\","
        + "        \"values\": ["
        + "          \"O\""
        + "        ]"
        + "      },"
        + "      {"
        + "        \"id\": 200,"
        + "        \"name\": \"役職\","
        + "        \"values\": ["
        + "          \"部長\","
        + "          \"マネージャー\""
        + "        ]"
        + "      }"
        + "    ]"
        + "  }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var members = await sut.FetchMembersDataAsync().ConfigureAwait(false);

        // Assert
        _ = members.Should().HaveCount(2);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/members"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AddMemberDataAsync"/>は、"/members"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.AddMemberDataAsync)} > POST /members をコールする。")]
    public async Task AddMemberDataAsync_Calls_PostApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.AddMemberDataAsync(_memberDataPayload).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(1);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Post);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/members"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.ReplaceMemberDataAsync"/>は、"/members"にPUTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.ReplaceMemberDataAsync)} > PUT /members をコールする。")]
    public async Task ReplaceMemberDataAsync_Calls_PutApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.ReplaceMemberDataAsync(_memberDataPayload).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(1);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Put);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/members"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.UpdateMemberDataAsync"/>は、"/members"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.UpdateMemberDataAsync)} > PATCH /members をコールする。")]
    public async Task UpdateMemberDataAsync_Calls_PatchApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.UpdateMemberDataAsync(_memberDataPayload).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(1);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Patch);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/members"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.OverWriteMemberDataAsync"/>は、"/members/overwrite"にPUTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.OverWriteMemberDataAsync)} > PUT /members/overwrite をコールする。")]
    public async Task OverWriteMemberDataAsync_Calls_PutApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.OverWriteMemberDataAsync(_memberDataPayload).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(1);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Put);
            _ = req.RequestUri?.PathAndQuery.Should().Be("/members/overwrite");

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.DeleteMemberDataAsync"/>は、"/members/delete"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.DeleteMemberDataAsync)} > POST /members/delete をコールする。")]
    public async Task DeleteMemberDataAsync_Calls_PostApi()
    {
        // Arrange
        string[] codes = _memberDataPayload.Select(d => d.Code).ToArray();
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"codes\":{JsonSerializer.Serialize(codes, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/delete")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.DeleteMemberDataAsync(codes).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(1);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Post);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/members/delete"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }
    #endregion メンバー情報 API

    #region シート情報 API
    /// <summary>Sheet APIのリクエストPayload</summary>
    private static readonly SheetData[] _sheetDataPayload = new SheetData[]
    {
            new (
                "A0002",
                new CustomFieldValue[]
                {
                    new(1000, "東京都港区x-x-x")
                }
            ),
            new (
                "A0001",
                new CustomFieldValue[]
                {
                    new(1000, "大阪府大阪市y番y号"),
                    new(1001, "06-yyyy-yyyy")
                }
                ,
                new CustomFieldValue[]
                {
                    new(1000, "愛知県名古屋市z丁目z番z号"),
                    new(1001, "052-zzzz-zzzz")
                }
            )
    };

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchSheetDataListAsync"/>は、"/sheets/{sheetId}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchSheetDataListAsync)} > GET /sheets/:sheetId をコールする。")]
    public async Task FetchSheetDataListAsync_Calls_GetApi()
    {
        // Arrange
        const int sheetId = 1;
        #region JSON
        const string responseJson = "{"
        + "  \"id\": 12,"
        + "  \"name\": \"住所・連絡先\","
        + "  \"record_type\": 1,"
        + "  \"updated_at\": \"2020-10-01 01:23:45\","
        + "  \"member_data\": ["
        + "    {"
        + "      \"code\": \"A0002\","
        + "      \"records\": ["
        + "        {"
        + "          \"custom_fields\": ["
        + "            {"
        + "              \"id\": 1000,"
        + "              \"name\": \"住所\","
        + "              \"values\": ["
        + "                \"東京都港区x-x-x\""
        + "              ]"
        + "            }"
        + "          ]"
        + "        }"
        + "      ]"
        + "    },"
        + "    {"
        + "      \"code\": \"A0001\","
        + "      \"records\": ["
        + "        {"
        + "          \"custom_fields\": ["
        + "            {"
        + "              \"id\": 1000,"
        + "              \"name\": \"住所\","
        + "              \"values\": ["
        + "                \"大阪府大阪市y番y号\""
        + "              ]"
        + "            },"
        + "            {"
        + "              \"id\": 1001,"
        + "              \"name\": \"電話番号\","
        + "              \"values\": ["
        + "                \"06-yyyy-yyyy\""
        + "              ]"
        + "            }"
        + "          ]"
        + "        },"
        + "        {"
        + "          \"custom_fields\": ["
        + "            {"
        + "              \"id\": 1000,"
        + "              \"name\": \"住所\","
        + "              \"values\": ["
        + "                \"愛知県名古屋市z丁目z番z号\""
        + "              ]"
        + "            },"
        + "            {"
        + "              \"id\": 1001,"
        + "              \"name\": \"電話番号\","
        + "              \"values\": ["
        + "                \"052-zzzz-zzzz\""
        + "              ]"
        + "            }"
        + "          ]"
        + "        }"
        + "      ]"
        + "    }"
        + "  ]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var members = await sut.FetchSheetDataListAsync(sheetId).ConfigureAwait(false);

        // Assert
        _ = members.Should().HaveCount(2);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/sheets/{sheetId}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.ReplaceSheetDataAsync"/>は、"/sheets/{sheetId}"にPUTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.ReplaceSheetDataAsync)} > PUT /sheets/:sheetId をコールする。")]
    public async Task ReplaceSheetDataAsync_Calls_PutApi()
    {
        // Arrange
        const int sheetId = 1;
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_sheetDataPayload, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.ReplaceSheetDataAsync(sheetId, _sheetDataPayload).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(sheetId);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Put);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/sheets/{sheetId}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.UpdateSheetDataAsync"/>は、"/sheets/{sheetId}"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.UpdateSheetDataAsync)} > PATCH /sheets/:sheetId をコールする。")]
    public async Task UpdateSheetDataAsync_Calls_PatchApi()
    {
        // Arrange
        const int sheetId = 1;
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_sheetDataPayload, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.UpdateSheetDataAsync(sheetId, _sheetDataPayload).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(sheetId);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Patch);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/sheets/{sheetId}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AddSheetDataAsync"/>は、"/sheets/{sheetId}/add"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.AddSheetDataAsync)} > POST /sheets/:sheetId/add をコールする。")]
    public async Task AddSheetDataAsync_Calls_PostApi()
    {
        // Arrange
        const int sheetId = 1;
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_sheetDataPayload, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}/add")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.AddSheetDataAsync(sheetId, _sheetDataPayload).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(sheetId);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Post);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/sheets/{sheetId}/add"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }
    #endregion シート情報 API

    #region 所属ツリー API
    /// <summary>
    /// <see cref="KaonaviV2Service.FetchDepartmentsAsync"/>は、"/departments"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchDepartmentsAsync)} > GET /departments をコールする。")]
    public async Task FetchDepartmentsAsync_Calls_GetApi()
    {
        // Arrange
        #region JSON
        const string responseJson = "{"
        + "\"department_data\": ["
        + "  {"
        + "    \"code\": \"1000\","
        + "    \"name\": \"取締役会\","
        + "    \"parent_code\": null,"
        + "    \"leader_member_code\": \"A0002\","
        + "    \"order\": 1,"
        + "    \"memo\": \"\""
        + "  },"
        + "  {"
        + "    \"code\": \"1200\","
        + "    \"name\": \"営業本部\","
        + "    \"parent_code\": null,"
        + "    \"leader_member_code\": null,"
        + "    \"order\": 2,"
        + "    \"memo\": \"\""
        + "  },"
        + "  {"
        + "    \"code\": \"1500\","
        + "    \"name\": \"第一営業部\","
        + "    \"parent_code\": \"1200\","
        + "    \"leader_member_code\": null,"
        + "    \"order\": 1,"
        + "    \"memo\": \"\""
        + "  },"
        + "  {"
        + "    \"code\": \"2000\","
        + "    \"name\": \"ITグループ\","
        + "    \"parent_code\": \"1500\","
        + "    \"leader_member_code\": \"A0001\","
        + "    \"order\": 1,"
        + "    \"memo\": \"example\""
        + "  }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/departments")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var departments = await sut.FetchDepartmentsAsync().ConfigureAwait(false);

        // Assert
        _ = departments.Should().HaveCount(4);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/departments"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.ReplaceDepartmentsAsync"/>は、"/departments"にPUTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.ReplaceDepartmentsAsync)} > PUT /departments をコールする。")]
    public async Task ReplaceDepartmentsAsync_Calls_PutApi()
    {
        // Arrange
        const int sheetId = 1;
        var payload = new DepartmentTree[]
        {
                new("1000", "取締役会", null, "A0002", 1, ""),
                new("1200", "営業本部", null, null, 2, ""),
                new("1500", "第一営業部", "1200", null, 1, ""),
                new("2000", "ITグループ", "1500", "A0001", 1, "example"),
        };
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"department_data\":{JsonSerializer.Serialize(payload, JsonConfig.Default)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/departments")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.ReplaceDepartmentsAsync(payload).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(sheetId);

        handler.VerifyRequest(async (req) =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Put);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/departments"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }
    #endregion 所属ツリー API

    #region ユーザー情報 API
    /// <summary>
    /// <see cref="KaonaviV2Service.FetchUsersAsync"/>は、"/users"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchUsersAsync)} > GET /users をコールする。")]
    public async Task FetchUsersAsync_Calls_GetApi()
    {
        // Arrange
        #region JSON
        const string responseJson = "{"
        + "\"user_data\": ["
        + "  {"
        + "    \"id\": 1,"
        + "    \"email\": \"taro@kaonavi.jp\","
        + "    \"member_code\": \"A0002\","
        + "    \"role\": {"
        + "      \"id\": 1,"
        + "      \"name\": \"システム管理者\","
        + "      \"type\": \"Adm\""
        + "    },"
        + "    \"last_login_at\": \"2021-11-01 12:00:00\""
        + "  },"
        + "  {"
        + "    \"id\": 2,"
        + "    \"email\": \"hanako@kaonavi.jp\","
        + "    \"member_code\": \"A0001\","
        + "    \"role\": {"
        + "      \"id\": 2,"
        + "      \"name\": \"マネージャ\","
        + "      \"type\": \"一般\""
        + "    },"
        + "    \"last_login_at\": null"
        + "  }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/users")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var users = await sut.FetchUsersAsync().ConfigureAwait(false);

        // Assert
        _ = users.Should().AllBeAssignableTo<UserWithLoginAt>()
            .And.HaveCount(2);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = req.RequestUri?.PathAndQuery.Should().Be("/users");

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AddUserAsync"/>は、"/users"にPOSTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.AddUserAsync)} > POST /users をコールする。")]
    public async Task AddUserAsync_Calls_PostApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        const string responseJson = "{"
        + "\"id\": 1,"
        + "\"email\": \"user1@example.com\","
        + "\"member_code\": \"00001\","
        + "\"role\": {"
        + "  \"id\": 1,"
        + "  \"name\": \"システム管理者\","
        + "  \"type\": \"Adm\""
        + "}"
        + "}";
        var payload = new UserPayload("user1@example.com", "00001", "password", 1);
        const string expectedJson = "{\"email\":\"user1@example.com\","
        + "\"member_code\":\"00001\","
        + "\"password\":\"password\","
        + "\"role\":{\"id\":1}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/users")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var user = await sut.AddUserAsync(payload).ConfigureAwait(false);

        // Assert
        _ = user.Should().NotBeNull();

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Post);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/users"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <inheritdoc cref="KaonaviV2Service.FetchUserAsync" path="/param[@name='userId']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.FetchUserAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchUserAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    public async Task FetchUserAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        Func<Task> act = async () => _ = await sut.FetchUserAsync(-1).ConfigureAwait(false);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithMessage("*userId*");

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchUserAsync"/>は、"/users/{userId}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchUserAsync)} > GET /users/:userId をコールする。")]
    public async Task FetchUserAsync_Calls_GetApi()
    {
        // Arrange
        const int userId = 1;
        string tokenString = GenerateRandomString();
        var responseUser = new UserWithLoginAt(
            userId,
            "user1@example.com",
            "00001",
            new(1, "システム管理者", "Adm"),
            new(2021, 11, 1, 12, 0, 0)
        );

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/users/{userId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, responseUser, KaonaviV2Service.Options);

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var user = await sut.FetchUserAsync(userId).ConfigureAwait(false);

        // Assert
        _ = user.Should().Be(responseUser);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = req.RequestUri?.PathAndQuery.Should().Be($"/users/{userId}");

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <inheritdoc cref="KaonaviV2Service.UpdateUserAsync" path="/param[@name='userId']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.UpdateUserAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.UpdateUserAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    public async Task UpdateUserAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        Func<Task> act = async () => _ = await sut.UpdateUserAsync(-1, null!).ConfigureAwait(false);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithMessage("*userId*")
            .ConfigureAwait(false);

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.UpdateUserAsync"/>は、"/users/{userId}"にPATCHリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.UpdateUserAsync)} > PATCH /users/:userId をコールする。")]
    public async Task UpdateUserAsync_Calls_PatchApi()
    {
        // Arrange
        const int userId = 1;
        const string responseJson = "{"
        + "\"id\": 1,"
        + "\"email\": \"user1@example.com\","
        + "\"member_code\": \"00001\","
        + "\"role\": {"
        + "  \"id\": 1,"
        + "  \"name\": \"システム管理者\","
        + "  \"type\": \"Adm\""
        + "}"
        + "}";
        string tokenString = GenerateRandomString();
        var payload = new UserPayload("user1@example.com", "00001", "password", 1);
        const string expectedJson = "{\"email\":\"user1@example.com\","
        + "\"member_code\":\"00001\","
        + "\"password\":\"password\","
        + "\"role\":{\"id\":1}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/users/{userId}")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var user = await sut.UpdateUserAsync(userId, payload).ConfigureAwait(false);

        // Assert
        _ = user.Should().NotBeNull();

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Patch);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/users/{userId}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <inheritdoc cref="KaonaviV2Service.DeleteUserAsync" path="/param[@name='userId']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.DeleteUserAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.DeleteUserAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    public async Task DeleteUserAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        Func<Task> act = async () => await sut.DeleteUserAsync(-1).ConfigureAwait(false);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithMessage("*userId*")
            .ConfigureAwait(false);

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.DeleteUserAsync"/>は、"/users/{userId}"にDELETEリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.DeleteUserAsync)} > DELETE /users/:userId をコールする。")]
    public async Task DeleteUserAsync_Calls_DeleteApi()
    {
        // Arrange
        const int userId = 1;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/users/{userId}")
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        await sut.DeleteUserAsync(userId).ConfigureAwait(false);

        // Assert
        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Delete);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/users/{userId}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }
    #endregion ユーザー情報 API

    #region ロール API
    /// <summary>
    /// <see cref="KaonaviV2Service.FetchRolesAsync"/>は、"/roles"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchRolesAsync)} > GET /roles をコールする。")]
    public async Task FetchRolesAsync_Calls_GetApi()
    {
        // Arrange
        #region JSON
        const string responseJson = "{"
        + "\"role_data\": ["
        + "  {"
        + "    \"id\": 1,"
        + "    \"name\": \"カオナビ管理者\","
        + "    \"type\": \"Adm\""
        + "  },"
        + "  {"
        + "    \"id\": 2,"
        + "    \"name\": \"カオナビマネージャー\","
        + "    \"type\": \"一般\""
        + "  }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/roles")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var roles = await sut.FetchRolesAsync().ConfigureAwait(false);

        // Assert
        _ = roles.Should().HaveCount(2);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/roles"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }
    #endregion ロール API

    #region マスター管理 API
    /// <summary>
    /// <see cref="KaonaviV2Service.FetchEnumOptionsAsync"/>は、"/enum_options"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchEnumOptionsAsync)} > GET /enum_options をコールする。")]
    public async Task FetchEnumOptionsAsync_Calls_GetApi()
    {
        // Arrange
        #region JSON
        const string responseJson = "{"
        + "\"custom_field_data\": ["
        + "  {"
        + "    \"sheet_name\": \"役職情報\","
        + "    \"id\": 10,"
        + "    \"name\": \"役職\","
        + "    \"enum_option_data\": ["
        + "      { \"id\": 1, \"name\": \"社長\" },"
        + "      { \"id\": 2, \"name\": \"部長\" },"
        + "      { \"id\": 3, \"name\": \"課長\" }"
        + "    ]"
        + "  },"
        + "  {"
        + "    \"sheet_name\": \"家族情報\","
        + "    \"id\": 20,"
        + "    \"name\": \"続柄区分\","
        + "    \"enum_option_data\": ["
        + "      { \"id\": 4, \"name\": \"父\" },"
        + "      { \"id\": 5, \"name\": \"母\" },"
        + "      { \"id\": 6, \"name\": \"兄\" },"
        + "      { \"id\": 7, \"name\": \"姉\" }"
        + "    ]"
        + "  },"
        + "  {"
        + "    \"sheet_name\": \"学歴情報\","
        + "    \"id\": 30,"
        + "    \"name\": \"学歴区分\","
        + "    \"enum_option_data\": ["
        + "      { \"id\": 8, \"name\": \"高校\" },"
        + "      { \"id\": 9, \"name\": \"大学\" },"
        + "      { \"id\": 10, \"name\": \"大学院\" }"
        + "    ]"
        + "  }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/enum_options")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var entities = await sut.FetchEnumOptionsAsync().ConfigureAwait(false);

        // Assert
        _ = entities.Should().HaveCount(3);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/enum_options"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchEnumOptionAsync"/>は、"/enum_options/{id}"にGETリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.FetchEnumOptionAsync)} > GET /enum_options/:id をコールする。")]
    public async Task FetchEnumOptionAsync_Calls_GetApi()
    {
        // Arrange
        #region JSON
        const string responseJson = "{"
        + "\"sheet_name\": \"役職情報\","
        + "\"id\": 10,"
        + "\"name\": \"役職\","
        + "\"enum_option_data\": ["
        + "  { \"id\": 1, \"name\": \"社長\" },"
        + "  { \"id\": 2, \"name\": \"部長\" },"
        + "  { \"id\": 3, \"name\": \"課長\" }"
        + "]"
        + "}";
        #endregion JSON
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/enum_options/10")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var entity = await sut.FetchEnumOptionAsync(10).ConfigureAwait(false);

        // Assert
        _ = entity.Should().NotBeNull();

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/enum_options/10"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.UpdateEnumOptionAsync"/>は、"/enum_options/{id}"にPUTリクエストを行う。
    /// </summary>
    [Fact(DisplayName = $"{nameof(KaonaviV2Service)} > {nameof(KaonaviV2Service.UpdateEnumOptionAsync)} > PUT /enum_options/:id をコールする。")]
    public async Task UpdateEnumOptionAsync_Calls_PutApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/enum_options/10")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.UpdateEnumOptionAsync(10, new (int?, string)[]
        {
                (1, "value1"),
                (null, "value2"),
        }).ConfigureAwait(false);

        // Assert
        _ = taskId.Should().Be(1);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Put);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/enum_options/10"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false);
            _ = receivedJson.Should().Be("{\"enum_option_data\":["
            + "{\"id\":1,\"name\":\"value1\"},"
            + "{\"name\":\"value2\"}"
            + "]}");

            return true;
        }, Times.Once());
    }
    #endregion マスター管理 API
}
