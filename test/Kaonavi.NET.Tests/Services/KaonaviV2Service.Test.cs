using System.Text;
using Kaonavi.Net.Api;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;
using Kaonavi.Net.Services;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests.Services;

/// <summary><see cref="KaonaviV2Service"/>の単体テスト</summary>
[TestClass]
public sealed class KaonaviV2ServiceTest
{
    /// <summary>テスト用の<see cref="HttpClient.BaseAddress"/></summary>
    private static readonly Uri _baseUri = new("https://example.com/");

    /// <summary>タスク結果JSON</summary>
    /*lang=json,strict*/
    private const string TaskJson = """{"task_id":1}""";

    /// <summary>ランダムな文字列を生成します。</summary>
    private static string GenerateRandomString() => Guid.NewGuid().ToString();

    #region Constructor
    /// <summary>
    /// コンストラクターを呼び出す<see cref="Action"/>を生成します。
    /// </summary>
    /// <param name="client"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='client']"/></param>
    /// <param name="consumerKey"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='consumerKey']"/></param>
    /// <param name="consumerSecret"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></param>
    private static Action Constructor(HttpClient? client, string? consumerKey, string? consumerSecret)
        => () => _ = new KaonaviV2Service(client!, consumerKey!, consumerSecret!);

    /// <summary>
    /// <paramref name="consumerKey"/>または<paramref name="consumerSecret"/>が<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    /// <param name="consumerKey"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='consumerKey']"/></param>
    /// <param name="consumerSecret"><inheritdoc cref="KaonaviV2Service(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></param>
    /// <param name="paramName">例外の原因となったパラメータ名</param>
    [TestMethod($"{nameof(KaonaviV2Service)}(constructor) > {nameof(ArgumentNullException)}をスローする。"), TestCategory("Constructor")]
    [DataRow(null, "foo", "consumerKey", DisplayName = $"{nameof(KaonaviV2Service)}({nameof(HttpClient)}, null, \"foo\") > {nameof(ArgumentNullException)}(consumerKey)をスローする。")]
    [DataRow("foo", null, "consumerSecret", DisplayName = $"{nameof(KaonaviV2Service)}({nameof(HttpClient)}, \"foo\", null) > {nameof(ArgumentNullException)}(consumerSecret)をスローする。")]
    public void WhenParamIsNull_Constructor_Throws_ArgumentNullException(string? consumerKey, string? consumerSecret, string paramName)
        => Constructor(new(), consumerKey, consumerSecret).Should().ThrowExactly<ArgumentNullException>().WithParameterName(paramName);

    /// <summary>
    /// HttpClientが<see langword="null"/>のとき、<see cref="ArgumentNullException"/>の例外をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service)}(null, \"foo\", \"bar\") > ArgumentNullException(client)をスローする。"), TestCategory("Constructor")]
    public void WhenClientIsNull_Constructor_Throws_ArgumentNullException()
        => Constructor(null, "foo", "bar").Should().ThrowExactly<ArgumentNullException>().WithParameterName("client");

    /// <summary>
    /// <see cref="HttpClient.BaseAddress"/>が<see langword="null"/>のとき、既定値をセットする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service)}(constructor) > {nameof(HttpClient.BaseAddress)}がnullのとき、既定値をセットする。"), TestCategory("Constructor")]
    public void Constructor_Sets_BaseAddress_WhenIsNull()
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
    [TestMethod($"{nameof(KaonaviV2Service)}(constructor) > {nameof(HttpClient.BaseAddress)}がnullでないときは、既定値をセットしない。"), TestCategory("Constructor")]
    public void Constructor_DoesNotSet_BaseAddress_WhenNotNull()
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
    #endregion Constructor

    #region Properties
    /// <summary>
    /// <see cref="KaonaviV2Service.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を返す。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.AccessToken)}(get) > Kaonavi-Tokenヘッダーの値を返す。"), TestCategory("Properties")]
    public void AccessToken_Returns_Header_KaonaviToken_Value()
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
    [TestMethod($"{nameof(KaonaviV2Service.AccessToken)}(get) > Kaonavi-Tokenヘッダーがないとき、nullを返す。"), TestCategory("Properties")]
    public void When_Header_KaonaviToken_IsNull_AccessToken_Returns_Null()
    {
        // Arrange - Act
        var sut = new KaonaviV2Service(new(), "foo", "bar");

        // Assert
        _ = sut.AccessToken.Should().BeNull();
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を設定する。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.AccessToken)}(set) > Kaonavi-Tokenヘッダーの値を設定する。"), TestCategory("Properties")]
    public void AccessToken_Sets_Header_KaonaviToken()
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
    [TestMethod($"{nameof(KaonaviV2Service.UseDryRun)}(get) > Dry-Run: 1 かどうかを返す。"), TestCategory("Properties")]
    [DataRow(null, false, DisplayName = $"{nameof(KaonaviV2Service.UseDryRun)} > Headerに Dry-Runの設定がない場合、 falseを返す。")]
    [DataRow("0", false, DisplayName = $"{nameof(KaonaviV2Service.UseDryRun)} > Headerに Dry-Run: 0 の設定がある場合、 falseを返す。")]
    [DataRow("1", true, DisplayName = $"{nameof(KaonaviV2Service.UseDryRun)} > Headerに Dry-Run: 1 の設定がある場合、 trueを返す。")]
    [DataRow("foo", false, DisplayName = $"{nameof(KaonaviV2Service.UseDryRun)} > Headerに Dry-Run: foo の設定がある場合、 falseを返す。")]
    public void UseDryRun_Returns_Header_DryRun_Is1(string? headerValue, bool expected)
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
    [TestMethod($"{nameof(KaonaviV2Service.UseDryRun)}(set) > Dry-Runヘッダーを追加/削除する。"), TestCategory("Properties")]
    public void UseDryRun_Sets_Header_DryRun()
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
        string key = GenerateRandomString();
        string secret = GenerateRandomString();
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/token")
            .ReturnsResponse(statusCode, responseBody, mediaType);

        // Act
        var sut = CreateSut(handler, key, secret);
        var act = async () => await sut.AuthenticateAsync();

        _ = (await act.Should().ThrowExactlyAsync<ApplicationException>())
            .WithMessage(message)
            .WithInnerExceptionExactly<HttpRequestException>();
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AccessToken"/>が<see langword="null"/>のとき、<see cref="KaonaviV2Service.AuthenticateAsync(CancellationToken)"/>を呼び出す。
    /// </summary>
    [TestMethod($"API Caller > {nameof(KaonaviV2Service.AuthenticateAsync)}を呼び出す。"), TestCategory("API")]
    public async Task When_AccessToken_IsNull_ApiCaller_Calls_AuthenticateAsync()
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
        var act = async () => await sut.FetchMemberLayoutAsync();

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ApplicationException>();

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

        // Act - Assert
        var sut = CreateSut(handler, accessToken: "token");
        _ = sut.UpdateRequestCount.Should().Be(0);

        // Normal calls (1-5)
        for (int i = 1; i <= 5; i++)
            await CallUpdateApiAndVerifyAsync(i);

        // Extra call (Wait 1 minute and reset call count)
        await CallUpdateApiAndVerifyAsync(1);

        handler.VerifyAnyRequest(Times.Exactly(6));

        async Task CallUpdateApiAndVerifyAsync(int expected)
        {
            _ = await sut.AddMemberDataAsync(_memberDataPayload);
            _ = sut.UpdateRequestCount.Should().Be(expected);
        }
    }

    /// <summary>
    /// 更新リクエスト制限の対象となるAPIは、エラー発生時に実行回数としてカウントされない。
    /// </summary>
    [TestMethod($"API Caller > 更新リクエスト制限の対象となるAPIは、エラー発生時に実行回数としてカウントされない。"), TestCategory("API")]
    public async Task UpdateApi_DoesNotWaits_WhenInvalid()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
            .ReturnsResponse(HttpStatusCode.NotFound, /*lang=json,strict*/ """{"errors":["test"]}""", "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: "token");
        var act = async () => await sut.OverWriteMemberDataAsync(_memberDataPayload);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ApplicationException>();
        _ = sut.UpdateRequestCount.Should().Be(0);
    }
    #endregion API Common Path

    /// <summary>
    /// <see cref="KaonaviV2Service.AuthenticateAsync"/>は、"/token"にBase64文字列のPOSTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.AuthenticateAsync)} > POST /token をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("アクセストークン")]
    public async Task AuthenticateAsync_Calls_PostApi()
    {
        // Arrange
        string key = GenerateRandomString();
        string secret = GenerateRandomString();
        string tokenString = GenerateRandomString();
        var response = new Token("25396f58-10f8-c228-7f0f-818b1d666b2e", "Bearer", 3600);

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/token")
            .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

        // Act
        var sut = CreateSut(handler, key, secret);
        var token = await sut.AuthenticateAsync();

        // Assert
        _ = token.Should().Be(response);

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
            string body = await req.Content!.ReadAsStringAsync();
            _ = body.Should().Be("grant_type=client_credentials");

            return true;
        }, Times.Once());
    }

    #region タスク進捗状況 API
    /// <summary>
    /// <inheritdoc cref="KaonaviV2Service.FetchTaskProgressAsync" path="/param[@name='taskId']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.FetchTaskProgressAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.FetchTaskProgressAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("タスク進捗状況")]
    public async Task When_TaskId_IsLowerThan0_FetchTaskProgressAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        var act = async () => _ = await sut.FetchTaskProgressAsync(-1);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>().WithParameterName("taskId");

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchTaskProgressAsync"/>は、"/tasks/{taskId}"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.FetchTaskProgressAsync)} > GET /tasks/:taskId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("タスク進捗状況")]
    public async Task FetchTaskProgressAsync_Calls_GetApi()
    {
        // Arrange
        const int taskId = 1;
        string tokenString = GenerateRandomString();
        var response = new TaskProgress(taskId, "NG", ["エラーメッセージ1", "エラーメッセージ2"]);

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/tasks/{taskId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var task = await sut.FetchTaskProgressAsync(taskId);

        // Assert
        _ = task.Should().NotBeNull();
        _ = task.Id.Should().Be(taskId);
        _ = task.Status.Should().Be("NG");
        _ = task.Messages.Should().Equal("エラーメッセージ1", "エラーメッセージ2");

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
    [TestMethod($"{nameof(KaonaviV2Service.FetchMemberLayoutAsync)} > GET /member_layouts をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
    public async Task FetchMemberLayoutAsync_Calls_GetApi()
    {
        string tokenString = GenerateRandomString();
        var response = new MemberLayout(
            new FieldLayout("社員番号", true, FieldType.String, 50, []),
            new FieldLayout("氏名", false, FieldType.String, 100, []),
            new FieldLayout("フリガナ", false, FieldType.String, 100, []),
            new FieldLayout("メールアドレス", false, FieldType.String, 100, []),
            new FieldLayout("入社日", false, FieldType.Date, null, []),
            new FieldLayout("退職日", false, FieldType.Date, null, []),
            new FieldLayout("性別", false, FieldType.Enum, null, ["男性", "女性"]),
            new FieldLayout("生年月日", false, FieldType.Date, null, []),
            new FieldLayout("所属", false, FieldType.Department, null, []),
            new FieldLayout("兼務情報", false, FieldType.DepartmentArray, null, []),
            [
                new(100, "血液型", false, FieldType.Enum, null, ["A", "B", "O", "AB"]),
                new(200, "役職", false, FieldType.Enum, null, ["部長", "課長", "マネージャー", null]),
            ]
        );

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/member_layouts")
            .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var layout = await sut.FetchMemberLayoutAsync();

        // Assert
        _ = layout.Should().NotBeNull();
        _ = layout.Code.Name.Should().Be("社員番号");
        _ = layout.Name.Name.Should().Be("氏名");
        _ = layout.NameKana.Name.Should().Be("フリガナ");
        _ = layout.Mail.Name.Should().Be("メールアドレス");
        _ = layout.EnteredDate.Name.Should().Be("入社日");
        _ = layout.RetiredDate.Name.Should().Be("退職日");
        _ = layout.Gender.Name.Should().Be("性別");
        _ = layout.Birthday.Name.Should().Be("生年月日");
        _ = layout.Department.Name.Should().Be("所属");
        _ = layout.SubDepartments.Name.Should().Be("兼務情報");
        _ = layout.CustomFields.Should().HaveCount(2);

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
    [TestMethod($"{nameof(KaonaviV2Service.FetchSheetLayoutsAsync)} > GET /sheet_layouts をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
    public async Task FetchSheetLayoutsAsync_Calls_GetApi()
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
            "sheets": [
                {
                    "id": 12,
                    "name": "住所・連絡先",
                    "record_type": 1,
                    "custom_fields": [
                        {
                            "id": 1000,
                            "name": "住所",
                            "required": false,
                            "type": "string",
                            "max_length": 250,
                            "enum": []
                        },
                        {
                            "id": 1001,
                            "name": "電話番号",
                            "required": false,
                            "type": "string",
                            "max_length": 50,
                            "enum": []
                        }
                    ]
                }
            ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/sheet_layouts")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var layouts = await sut.FetchSheetLayoutsAsync();

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
    [TestMethod($"{nameof(KaonaviV2Service.FetchSheetLayoutsAsync)} > GET /sheet_layouts/:sheetId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("レイアウト設定")]
    public async Task FetchSheetLayoutAsync_Calls_GetApi()
    {
        // Arrange
        const int sheetId = 12;
        string tokenString = GenerateRandomString();
        var response = new SheetLayout(
            sheetId,
            "住所・連絡先",
            RecordType.Multiple,
            [
                new(1000, "住所", false, FieldType.String, 250, []),
                new(1001, "電話番号", false, FieldType.String, 50, []),
            ]
        );

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheet_layouts/{sheetId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, response, Context.Default.Options);

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var layout = await sut.FetchSheetLayoutAsync(12);

        // Assert
        _ = layout.Should().NotBeNull();
        _ = layout.Id.Should().Be(sheetId);
        _ = layout.Name.Should().Be("住所・連絡先");
        _ = layout.RecordType.Should().Be(RecordType.Multiple);
        _ = layout.CustomFields.Should().HaveCount(2);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/sheet_layouts/{sheetId}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").Should().Equal(tokenString);

            return true;
        }, Times.Once());
    }
    #endregion レイアウト設定 API

    #region メンバー情報 API
    /// <summary>Member APIのリクエストPayload</summary>
    private static readonly MemberData[] _memberDataPayload =
    [
        new(
            Code: "A0002",
            Name: "カオナビ 太郎",
            NameKana: "カオナビ タロウ",
            Mail: "taro@example.com",
            EnteredDate: new(2005, 9, 20),
            Gender: "男性",
            Birthday: new(1984, 5, 15),
            Department: new("1000"),
            SubDepartments: [],
            CustomFields: [new(100, "A")]
        ),
        new(
            Code: "A0001",
            Name: "カオナビ 花子",
            NameKana: "カオナビ ハナコ",
            Mail: "hanako@kaonavi.jp",
            EnteredDate: new(2013, 5, 7),
            Gender: "女性",
            Birthday: new(1986, 5, 16),
            Department: new("2000"),
            SubDepartments:
            [
                new("3000"),
                new("4000")
            ],
            CustomFields:
            [
                new(100, "O"),
                new(200, ["部長", "マネージャー"])
            ]
        )
    ];

    /// <summary>
    /// <see cref="KaonaviV2Service.FetchMembersDataAsync"/>は、"/members"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.FetchMembersDataAsync)} > GET /members をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("メンバー情報")]
    public async Task FetchMembersDataAsync_Calls_GetApi()
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
            "updated_at": "2020-10-01 01:23:45",
            "member_data": [
                {
                    "code": "A0002",
                    "name": "カオナビ 太郎",
                    "name_kana": "カオナビ タロウ",
                    "mail": "taro@kaonavi.jp",
                    "entered_date": "2005-09-20",
                    "retired_date": "",
                    "gender": "男性",
                    "birthday": "1984-05-15",
                    "age": 36,
                    "years_of_service": "15年5ヵ月",
                    "department": {
                        "code": "1000",
                        "name": "取締役会",
                        "names": ["取締役会"]
                    },
                    "sub_departments": [],
                    "custom_fields": [
                        {
                            "id": 100,
                            "name": "血液型",
                            "values": ["A"]
                        }
                    ]
                },
                {
                    "code": "A0001",
                    "name": "カオナビ 花子",
                    "name_kana": "カオナビ ハナコ",
                    "mail": "hanako@kaonavi.jp",
                    "entered_date": "2013-05-07",
                    "retired_date": "",
                    "gender": "女性",
                    "birthday": "1986-05-16",
                    "age": 36,
                    "years_of_service": "7年9ヵ月",
                    "department": {
                        "code": "2000",
                        "name": "営業本部 第一営業部 ITグループ",
                        "names": ["営業本部", "第一営業部", "ITグループ"]
                    },
                    "sub_departments": [
                        {
                            "code": "3000",
                            "name": "企画部",
                            "names": ["企画部"]
                        },
                        {
                            "code": "4000",
                            "name": "管理部",
                            "names": ["管理部"]
                        }
                    ],
                    "custom_fields": [
                        {
                            "id": 100,
                            "name": "血液型",
                            "values": ["O"]
                        },
                        {
                            "id": 200,
                            "name": "役職",
                            "values": ["部長", "マネージャー"]
                        }
                    ]
                }
            ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var members = await sut.FetchMembersDataAsync();

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
    [TestMethod($"{nameof(KaonaviV2Service.AddMemberDataAsync)} > POST /members をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("メンバー情報")]
    public async Task AddMemberDataAsync_Calls_PostApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, Context.Default.IReadOnlyCollectionMemberData)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.AddMemberDataAsync(_memberDataPayload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.ReplaceMemberDataAsync"/>は、"/members"にPUTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.ReplaceMemberDataAsync)} > PUT /members をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("メンバー情報")]
    public async Task ReplaceMemberDataAsync_Calls_PutApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, Context.Default.IReadOnlyCollectionMemberData)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.ReplaceMemberDataAsync(_memberDataPayload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.UpdateMemberDataAsync"/>は、"/members"にPATCHリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.UpdateMemberDataAsync)} > PATCH /members をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("メンバー情報")]
    public async Task UpdateMemberDataAsync_Calls_PatchApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, Context.Default.IReadOnlyCollectionMemberData)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.UpdateMemberDataAsync(_memberDataPayload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.OverWriteMemberDataAsync"/>は、"/members/overwrite"にPUTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.OverWriteMemberDataAsync)} > PUT /members/overwrite をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("メンバー情報")]
    public async Task OverWriteMemberDataAsync_Calls_PutApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, Context.Default.IReadOnlyCollectionMemberData)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/overwrite")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.OverWriteMemberDataAsync(_memberDataPayload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.DeleteMemberDataAsync"/>は、"/members/delete"にPOSTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.DeleteMemberDataAsync)} > POST /members/delete をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("メンバー情報")]
    public async Task DeleteMemberDataAsync_Calls_PostApi()
    {
        // Arrange
        string[] codes = _memberDataPayload.Select(d => d.Code).ToArray();
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"codes\":{JsonSerializer.Serialize(codes, Context.Default.IReadOnlyCollectionString)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/members/delete")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.DeleteMemberDataAsync(codes);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }
    #endregion メンバー情報 API

    #region シート情報 API
    /// <summary>Sheet APIのリクエストPayload</summary>
    private static readonly SheetData[] _sheetDataPayload =
    [
        new(
            "A0002",
            [new CustomFieldValue(1000, "東京都港区x-x-x")]
        ),
        new(
            "A0001",
            [
                new(1000, "大阪府大阪市y番y号"),
                new(1001, "06-yyyy-yyyy")
            ]
            ,
            [
                new(1000, "愛知県名古屋市z丁目z番z号"),
                new(1001, "052-zzzz-zzzz")
            ]
        )
    ];

    /// <summary>
    /// <see cref="KaonaviV2Service.Sheet.ListAsync"/>は、"/sheets/{sheetId}"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Sheet)}.{nameof(KaonaviV2Service.Sheet.ListAsync)} > GET /sheets/:sheetId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("シート情報")]
    public async Task Sheet_ListAsync_Calls_GetApi()
    {
        // Arrange
        const int sheetId = 1;
        /*lang=json,strict*/
        const string responseJson = """
        {
          "id": 12,
          "name": "住所・連絡先",
          "record_type": 1,
          "updated_at": "2020-10-01 01:23:45",
          "member_data": [
            {
              "code": "A0002",
              "records": [
                {
                  "custom_fields": [
                    {
                      "id": 1000,
                      "name": "住所",
                      "values": ["東京都港区x-x-x"]
                    }
                  ]
                }
              ]
            },
            {
              "code": "A0001",
              "records": [
                {
                  "custom_fields": [
                    {
                      "id": 1000,
                      "name": "住所",
                      "values": ["大阪府大阪市y番y号"]
                    },
                    {
                      "id": 1001,
                      "name": "電話番号",
                      "values": ["06-yyyy-yyyy"]
                    }
                  ]
                },
                {
                  "custom_fields": [
                    {
                      "id": 1000,
                      "name": "住所",
                      "values": ["愛知県名古屋市z丁目z番z号"]
                    },
                    {
                      "id": 1001,
                      "name": "電話番号",
                      "values": ["052-zzzz-zzzz"]
                    }
                  ]
                }
              ]
            }
          ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var members = await sut.Sheet.ListAsync(sheetId);

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
    /// <see cref="KaonaviV2Service.Sheet.ReplaceAsync"/>は、"/sheets/{sheetId}"にPUTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Sheet)}.{nameof(KaonaviV2Service.Sheet.ReplaceAsync)} > PUT /sheets/:sheetId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("シート情報")]
    public async Task Sheet_ReplaceAsync_Calls_PutApi()
    {
        // Arrange
        const int sheetId = 1;
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_sheetDataPayload, Context.Default.IReadOnlyCollectionSheetData)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.Sheet.ReplaceAsync(sheetId, _sheetDataPayload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.Sheet.UpdateAsync"/>は、"/sheets/{sheetId}"にPATCHリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Sheet)}.{nameof(KaonaviV2Service.Sheet.UpdateAsync)} > PATCH /sheets/:sheetId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("シート情報")]
    public async Task Sheet_UpdateAsync_Calls_PatchApi()
    {
        // Arrange
        const int sheetId = 1;
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_sheetDataPayload, Context.Default.IReadOnlyCollectionSheetData)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.Sheet.UpdateAsync(sheetId, _sheetDataPayload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.Sheet.CreateAsync"/>は、"/sheets/{sheetId}/add"にPOSTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Sheet)}.{nameof(KaonaviV2Service.Sheet.CreateAsync)} > POST /sheets/:sheetId/add をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("シート情報")]
    public async Task Sheet_CreateAsync_Calls_PostApi()
    {
        // Arrange
        const int sheetId = 1;
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_sheetDataPayload, Context.Default.IReadOnlyCollectionSheetData)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/sheets/{sheetId}/add")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.Sheet.CreateAsync(sheetId, _sheetDataPayload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <inheritdoc cref="IUser.CreateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.Sheet.CreateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Sheet)}.{nameof(KaonaviV2Service.Sheet.CreateAsync)} > ArgumentOutOfRangeException をスローする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("シート情報")]
    public async Task WhenNegative_Sheet_CreateAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        var act = async () => _ = await sut.Sheet.CreateAsync(-1, _sheetDataPayload);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithParameterName("id");

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    #endregion シート情報 API

    #region 所属ツリー API
    /// <summary>
    /// <see cref="KaonaviV2Service.Department.ListAsync"/>は、"/departments"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Department)}.{nameof(KaonaviV2Service.Department.ListAsync)} > GET /departments をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("所属ツリー")]
    public async Task Department_ListAsync_Calls_GetApi()
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
          "department_data": [
            {
              "code": "1000",
              "name": "取締役会",
              "parent_code": null,
              "leader_member_code": "A0002",
              "order": 1,
              "memo": ""
            },
            {
              "code": "1200",
              "name": "営業本部",
              "parent_code": null,
              "leader_member_code": null,
              "order": 2,
              "memo": ""
            },
            {
              "code": "1500",
              "name": "第一営業部",
              "parent_code": "1200",
              "leader_member_code": null,
              "order": 1,
              "memo": ""
            },
            {
              "code": "2000",
              "name": "ITグループ",
              "parent_code": "1500",
              "leader_member_code": "A0001",
              "order": 1,
              "memo": "example"
            }
          ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/departments")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var departments = await sut.Department.ListAsync();

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
    /// <see cref="KaonaviV2Service.Department.ReplaceAsync"/>は、"/departments"にPUTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Department)}.{nameof(KaonaviV2Service.Department.ReplaceAsync)} > PUT /departments をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("所属ツリー")]
    public async Task Department_ReplaceAsync_Calls_PutApi()
    {
        // Arrange
        var payload = new DepartmentTree[]
        {
            new("1000", "取締役会", null, "A0002", 1, ""),
            new("1200", "営業本部", null, null, 2, ""),
            new("1500", "第一営業部", "1200", null, 1, ""),
            new("2000", "ITグループ", "1500", "A0001", 1, "example"),
        };
        string tokenString = GenerateRandomString();
        string expectedJson = $"{{\"department_data\":{JsonSerializer.Serialize(payload, Context.Default.IReadOnlyCollectionDepartmentTree)}}}";

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/departments")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.Department.ReplaceAsync(payload);

        // Assert
        _ = taskId.Should().Be(1);

        handler.VerifyRequest(async (req) =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Put);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/departments"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }
    #endregion 所属ツリー API

    #region ユーザー情報 API
    /// <summary>
    /// <see cref="KaonaviV2Service.User.ListAsync"/>は、"/users"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.User)}.{nameof(KaonaviV2Service.User.ListAsync)} > GET /users をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ユーザー情報")]
    public async Task User_ListAsync_Calls_GetApi()
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
          "user_data": [
            {
              "id": 1,
              "email": "taro@kaonavi.jp",
              "member_code": "A0002",
              "role": {
                "id": 1,
                "name": "システム管理者",
                "type": "Adm"
              },
              "last_login_at": "2021-11-01 12:00:00"
            },
            {
              "id": 2,
              "email": "hanako@kaonavi.jp",
              "member_code": "A0001",
              "role": {
                "id": 2,
                "name": "マネージャ",
                "type": "一般"
              },
              "last_login_at": null
            }
          ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/users")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var users = await sut.User.ListAsync();

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
    /// <see cref="KaonaviV2Service.User.CreateAsync"/>は、"/users"にPOSTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.User)}.{nameof(KaonaviV2Service.User.CreateAsync)} > POST /users をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("ユーザー情報")]
    public async Task User_CreateAsync_Calls_PostApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        /*lang=json,strict*/
        const string responseJson = """
        {
          "id": 1,
          "email": "user1@example.com",
          "member_code": "00001",
          "role": {
            "id": 1,
            "name": "システム管理者",
            "type": "Adm"
          }
        }
        """;
        var payload = new UserPayload("user1@example.com", "00001", "password", 1);
        /*lang=json,strict*/
        const string expectedJson = """
        {"email":"user1@example.com","member_code":"00001","password":"password","role":{"id":1}}
        """;

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/users")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var user = await sut.User.CreateAsync(payload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <inheritdoc cref="IUser.ReadAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.User.ReadAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.User)}.{nameof(KaonaviV2Service.User.ReadAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ユーザー情報")]
    public async Task When_UserId_IsLowerThan0_User_ReadAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        var act = async () => _ = await sut.User.ReadAsync(-1);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithParameterName("id");

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.User.ReadAsync"/>は、"/users/{userId}"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.User)}.{nameof(KaonaviV2Service.User.ReadAsync)} > GET /users/:userId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ユーザー情報")]
    public async Task User_ReadAsync_Calls_GetApi()
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
            .ReturnsJsonResponse(HttpStatusCode.OK, responseUser, Context.Default.Options);

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var user = await sut.User.ReadAsync(userId);

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
    /// <inheritdoc cref="IUser.UpdateAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.User.UpdateAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.User)}.{nameof(KaonaviV2Service.User.UpdateAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("ユーザー情報")]
    public async Task When_UserId_IsLowerThan0_User_UpdateAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        var act = async () => _ = await sut.User.UpdateAsync(-1, null!);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithParameterName("id");

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.User.UpdateAsync"/>は、"/users/{userId}"にPATCHリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.User)}.{nameof(KaonaviV2Service.User.UpdateAsync)} > PATCH /users/:userId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("ユーザー情報")]
    public async Task User_UpdateAsync_Calls_PatchApi()
    {
        // Arrange
        const int userId = 1;
        /*lang=json,strict*/
        const string responseJson = """
        {
          "id": 1,
          "email": "user1@example.com",
          "member_code": "00001",
          "role": {
            "id": 1,
            "name": "システム管理者",
            "type": "Adm"
          }
        }
        """;
        string tokenString = GenerateRandomString();
        var payload = new UserPayload("user1@example.com", "00001", "password", 1);
        /*lang=json,strict*/
        const string expectedJson = """
        {"email":"user1@example.com","member_code":"00001","password":"password","role":{"id":1}}
        """;

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/users/{userId}")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var user = await sut.User.UpdateAsync(userId, payload);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <inheritdoc cref="IUser.DeleteAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.User.DeleteAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.User)}.{nameof(KaonaviV2Service.User.DeleteAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Delete)), TestCategory("ユーザー情報")]
    public async Task When_UserId_IsLowerThan0_User_DeleteAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        var act = async () => await sut.User.DeleteAsync(-1);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithParameterName("id");

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.User.DeleteAsync"/>は、"/users/{userId}"にDELETEリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.User)}.{nameof(KaonaviV2Service.User.DeleteAsync)} > DELETE /users/:userId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Delete)), TestCategory("ユーザー情報")]
    public async Task User_DeleteAsync_Calls_DeleteApi()
    {
        // Arrange
        const int userId = 1;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/users/{userId}")
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        await sut.User.DeleteAsync(userId);

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
    /// <see cref="KaonaviV2Service.Role.ListAsync"/>は、"/roles"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Role)}.{nameof(KaonaviV2Service.Role.ListAsync)} > GET /roles をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("ロール")]
    public async Task Role_ListAsync_Calls_GetApi()
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
          "role_data": [
            {
              "id": 1,
              "name": "カオナビ管理者",
              "type": "Adm"
            },
            {
              "id": 2,
              "name": "カオナビマネージャー",
              "type": "一般"
            }
          ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/roles")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var roles = await sut.Role.ListAsync();

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

    #region 拡張アクセス設定 API
    /// <summary>
    /// <paramref name="type"/>が不正な値であるとき、<see cref="KaonaviV2Service.AdvancedPermission.ListAsync"/>は、ArgumentOutOfRangeExceptionをスローする。
    /// </summary>
    /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary/text()"/></param>
    [TestMethod($"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ListAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("拡張アクセス設定")]
    [DataRow(10, DisplayName = $"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ListAsync)}(({nameof(AdvancedType)})10) > ArgumentOutOfRangeExceptionをスローする。")]
    [DataRow(-1, DisplayName = $"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ListAsync)}(({nameof(AdvancedType)})-1) > ArgumentOutOfRangeExceptionをスローする。")]
    public async Task WhenInvalidType_AdvancedPermission_ListAsync_Throws_ArgumentOutOfRangeException(int type)
    {
        // Arrange
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var act = async () => await sut.AdvancedPermission.ListAsync((AdvancedType)type);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>().WithParameterName(nameof(type));
        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AdvancedPermission.ListAsync"/>は、"/advanced_permissions/:advancedType"にGETリクエストを行う。
    /// </summary>
    /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary/text()"/></param>
    /// <param name="endpoint">呼ばれるAPIエンドポイント</param>
    [TestMethod($"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ListAsync)} > GET /advanced_permissions/:advancedType をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("拡張アクセス設定")]
    [DataRow(AdvancedType.Member, "/advanced_permissions/member", DisplayName = $"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ListAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Member)}) > GET /advanced_permissions/member をコールする。")]
    [DataRow(AdvancedType.Department, "/advanced_permissions/department", DisplayName = $"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ListAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Department)}) > GET /advanced_permissions/department をコールする。")]
    public async Task AdvancedPermission_ListAsync_Calls_GetApi(AdvancedType type, string endpoint)
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
          "advanced_permission_data": [
            {
              "user_id": 1,
              "add_codes": [
                "0001",
                "0002",
                "0003"
              ],
              "exclusion_codes": [
                "0001",
                "0002",
                "0003"
              ]
            },
            {
              "user_id": 2,
              "add_codes": [
                "0001",
                "0002",
                "0003"
              ],
              "exclusion_codes": [
                "0001",
                "0002",
                "0003"
              ]
            }
          ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == endpoint)
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var permissions = await sut.AdvancedPermission.ListAsync(type);

        // Assert
        _ = permissions.Should().HaveCount(2);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be(endpoint));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.AdvancedPermission.ReplaceAsync"/>は、"/advanced_permissions/:advancedType"にPUTリクエストを行う。
    /// </summary>
    /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary/text()"/></param>
    /// <param name="endpoint">呼ばれるAPIエンドポイント</param>
    [TestMethod($"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ReplaceAsync)} > PUT /advanced_permissions/:advancedType をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("拡張アクセス設定")]
    [DataRow(AdvancedType.Member, "/advanced_permissions/member", DisplayName = $"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ReplaceAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Member)}) > PUT /advanced_permissions/member をコールする。")]
    [DataRow(AdvancedType.Department, "/advanced_permissions/department", DisplayName = $"{nameof(KaonaviV2Service.AdvancedPermission)}.{nameof(KaonaviV2Service.AdvancedPermission.ReplaceAsync)}({nameof(AdvancedType)}.{nameof(AdvancedType.Department)}) > PUT /advanced_permissions/department をコールする。")]
    public async Task AdvancedPermission_ReplaceAsync_Calls_PutApi(AdvancedType type, string endpoint)
    {
        // Arrange
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == endpoint)
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.AdvancedPermission.ReplaceAsync(type,
        [
            new(1, ["1"], []),
            new(2, ["2"], ["1", "3"]),
        ]);

        // Assert
        _ = taskId.Should().Be(1);

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Put);
            _ = (req.RequestUri?.PathAndQuery.Should().Be(endpoint));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should()
                .Be(/*lang=json,strict*/ """{"advanced_permission_data":[{"user_id":1,"add_codes":["1"],"exclusion_codes":[]},{"user_id":2,"add_codes":["2"],"exclusion_codes":["1","3"]}]}""");

            return true;
        }, Times.Once());
    }
    #endregion 拡張アクセス設定 API

    #region マスター管理 API
    /// <summary>
    /// <see cref="KaonaviV2Service.EnumOption.ListAsync"/>は、"/enum_options"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.EnumOption)}.{nameof(KaonaviV2Service.EnumOption.ListAsync)} > GET /enum_options をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("マスター管理")]
    public async Task EnumOption_ListAsync_Calls_GetApi()
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
          "custom_field_data": [
            {
              "sheet_name": "役職情報",
              "id": 10,
              "name": "役職",
              "enum_option_data": [
                { "id": 1, "name": "社長" },
                { "id": 2, "name": "部長" },
                { "id": 3, "name": "課長" }
              ]
            },
            {
              "sheet_name": "家族情報",
              "id": 20,
              "name": "続柄区分",
              "enum_option_data": [
                { "id": 4, "name": "父" },
                { "id": 5, "name": "母" },
                { "id": 6, "name": "兄" },
                { "id": 7, "name": "姉" }
              ]
            },
            {
              "sheet_name": "学歴情報",
              "id": 30,
              "name": "学歴区分",
              "enum_option_data": [
                { "id": 8, "name": "高校" },
                { "id": 9, "name": "大学" },
                { "id": 10, "name": "大学院" }
              ]
            }
          ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/enum_options")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var entities = await sut.EnumOption.ListAsync();

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
    /// <see cref="KaonaviV2Service.EnumOption.ReadAsync"/>は、"/enum_options/{id}"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.EnumOption)}.{nameof(KaonaviV2Service.EnumOption.ReadAsync)} > GET /enum_options/:id をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("マスター管理")]
    public async Task EnumOption_ReadAsync_Calls_GetApi()
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
          "sheet_name": "役職情報",
          "id": 10,
          "name": "役職",
          "enum_option_data": [
            { "id": 1, "name": "社長" },
            { "id": 2, "name": "部長" },
            { "id": 3, "name": "課長" }
          ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/enum_options/10")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var entity = await sut.EnumOption.ReadAsync(10);

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
    /// <see cref="KaonaviV2Service.EnumOption.UpdateAsync"/>は、"/enum_options/{id}"にPUTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.EnumOption)}.{nameof(KaonaviV2Service.EnumOption.UpdateAsync)} > PUT /enum_options/:id をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Put)), TestCategory("マスター管理")]
    public async Task EnumOption_UpdateAsync_Calls_PutApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/enum_options/10")
            .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        int taskId = await sut.EnumOption.UpdateAsync(10, [(1, "value1"), (null, "value2")]);

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
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should()
                .Be(/*lang=json,strict*/ """{"enum_option_data":[{"id":1,"name":"value1"},{"name":"value2"}]}""");

            return true;
        }, Times.Once());
    }
    #endregion マスター管理 API

    #region Webhook設定 API
    /// <summary>
    /// <see cref="KaonaviV2Service.Webhook.ListAsync"/>は、"/webhook"にGETリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Webhook)}.{nameof(KaonaviV2Service.Webhook.ListAsync)} > GET /webhook をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("Webhook設定")]
    public async Task Webhook_ListAsync_Calls_GetApi()
    {
        // Arrange
        /*lang=json,strict*/
        const string responseJson = """
        {
          "webhook_data": [
            {
              "id": 1,
              "url": "https://example.com/",
              "events": ["member_created","member_deleted"],
              "secret_token": "string",
              "updated_at": "2021-12-01 12:00:00",
              "created_at": "2021-11-01 12:00:00"
            },
            {
              "id": 2,
              "url": "https://example.com/",
              "events": ["member_updated"],
              "secret_token": "string",
              "updated_at": "2021-12-01 12:00:00",
              "created_at": "2021-11-01 12:00:00"
            }
          ]
        }
        """;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/webhook")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var entities = await sut.Webhook.ListAsync();

        // Assert
        _ = entities.Should().HaveCount(2);

        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Get);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/webhook"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.Webhook.CreateAsync"/>は、"/webhook"にPOSTリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Webhook)}.{nameof(KaonaviV2Service.Webhook.CreateAsync)} > POST /webhook をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("Webhook設定")]
    public async Task Webhook_CreateAsync_Calls_PostApi()
    {
        // Arrange
        string tokenString = GenerateRandomString();
        /*lang=json,strict*/
        const string responseJson = """
        {
          "id": 1,
          "url": "https://example.com/",
          "events": [
            "member_created",
            "member_updated",
            "member_deleted"
          ],
          "secret_token": "token"
        }
        """;
        var payload = new WebhookConfigPayload(_baseUri, [WebhookEvent.MemberCreated, WebhookEvent.MemberUpdated, WebhookEvent.MemberDeleted], "token");
        /*lang=json,strict*/
        const string expectedJson = """
        {"url":"https://example.com/","events":["member_created","member_updated","member_deleted"],"secret_token":"token"}
        """;

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/webhook")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var user = await sut.Webhook.CreateAsync(payload);

        // Assert
        _ = user.Should().NotBeNull();

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Post);
            _ = (req.RequestUri?.PathAndQuery.Should().Be("/webhook"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.Webhook.UpdateAsync"/>は、"/webhook/{userId}"にPATCHリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Webhook)}.{nameof(KaonaviV2Service.Webhook.UpdateAsync)} > PATCH /users/:userId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("Webhook設定")]
    public async Task Webhook_UpdateAsync_Calls_PatchApi()
    {
        // Arrange
        const int id = 1;
        /*lang=json,strict*/
        const string responseJson = """
        {
          "id": 1,
          "url": "https://example.com/",
          "events": [
            "member_created",
            "member_updated",
            "member_deleted"
          ],
          "secret_token": "token"
        }
        """;
        string tokenString = GenerateRandomString();
        var payload = new WebhookConfig(id, _baseUri, [WebhookEvent.MemberCreated, WebhookEvent.MemberUpdated, WebhookEvent.MemberDeleted], "token");
        /*lang=json,strict*/
        const string expectedJson = """
        {"id":1,"url":"https://example.com/","events":["member_created","member_updated","member_deleted"],"secret_token":"token"}
        """;

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/webhook/{id}")
            .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        var user = await sut.Webhook.UpdateAsync(payload);

        // Assert
        _ = user.Should().NotBeNull();

        handler.VerifyRequest(async req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Patch);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/webhook/{id}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            // Body
            string receivedJson = await req.Content!.ReadAsStringAsync();
            _ = receivedJson.Should().Be(expectedJson);

            return true;
        }, Times.Once());
    }

    /// <summary>
    /// <inheritdoc cref="IWebhook.DeleteAsync" path="/param[@name='webhookId']"/>が<c>0</c>未満のとき、
    /// <see cref="KaonaviV2Service.Webhook.DeleteAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Webhook)}.{nameof(KaonaviV2Service.Webhook.DeleteAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Delete)), TestCategory("Webhook設定")]
    public async Task When_WebhookId_IsLowerThan0_Webhook_DeleteAsync_Throws_ArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

        // Act
        var sut = CreateSut(handler);
        var act = async () => await sut.Webhook.DeleteAsync(-1);

        // Assert
        _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
            .WithParameterName("id");

        handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
    }

    /// <summary>
    /// <see cref="KaonaviV2Service.Webhook.DeleteAsync"/>は、"/webhook/{webhookId}"にDELETEリクエストを行う。
    /// </summary>
    [TestMethod($"{nameof(KaonaviV2Service.Webhook)}.{nameof(KaonaviV2Service.Webhook.DeleteAsync)} > DELETE /webhook/:webhookId をコールする。")]
    [TestCategory("API"), TestCategory(nameof(HttpMethod.Delete)), TestCategory("Webhook設定")]
    public async Task Webhook_DeleteAsync_Calls_DeleteApi()
    {
        // Arrange
        const int webhookId = 1;
        string tokenString = GenerateRandomString();

        var handler = new Mock<HttpMessageHandler>();
        _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/webhook/{webhookId}")
            .ReturnsResponse(HttpStatusCode.NoContent);

        // Act
        var sut = CreateSut(handler, accessToken: tokenString);
        await sut.Webhook.DeleteAsync(webhookId);

        // Assert
        handler.VerifyRequest(req =>
        {
            // End point
            _ = req.Method.Should().Be(HttpMethod.Delete);
            _ = (req.RequestUri?.PathAndQuery.Should().Be($"/webhook/{webhookId}"));

            // Header
            _ = req.Headers.GetValues("Kaonavi-Token").First().Should().Be(tokenString);

            return true;
        }, Times.Once());
    }
    #endregion Webhook設定 API
}
