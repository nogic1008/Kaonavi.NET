using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Entities.Api;
using Kaonavi.Net.Services;
using Moq;
using Moq.Contrib.HttpClient;
using Xunit;

namespace Kaonavi.Net.Tests.Services
{
    /// <summary>
    /// <see cref="KaonaviV2Service"/>の単体テスト
    /// </summary>
    public class KaonaviV2ServiceTest
    {
        private const string TestName = nameof(KaonaviV2Service) + " > ";

        /// <summary>テスト用の<see cref="HttpClient.BaseAddress"/></summary>
        private static readonly Uri _baseUri = new("https://example.com/");

        /// <summary>タスク結果JSON</summary>
        private const string TaskJson = "{\"task_id\":1}";

        /// <summary>ランダムな文字列を生成します。</summary>
        private static string GenerateRandomString() => Guid.NewGuid().ToString();

        #region Constractor
        private const string TestNameConstractor = TestName + nameof(Constractor) + " > ";
        /// <summary>
        /// コンストラクターを呼び出す<see cref="Action"/>を生成します。
        /// </summary>
        private static Action Constractor(HttpClient? client, string? consumerKey, string? consumerSecret)
            => () => _ = new KaonaviV2Service(client!, consumerKey!, consumerSecret!);

        /// <summary>
        /// Consumer KeyまたはConsumer Secretがnullのとき、<see cref="ArgumentNullException"/>の例外をスローする。
        /// </summary>
        /// <param name="consumerKey">Consumer Key</param>
        /// <param name="consumerSecret">Consumer Secret</param>
        [Theory(DisplayName = TestNameConstractor + nameof(ArgumentNullException) + "をスローする。")]
        [InlineData(null, "foo")]
        [InlineData("foo", null)]
        public void Constractor_Throws_ArgumentNullException_WhenKeyIsNull(string? consumerKey, string? consumerSecret)
            => Constractor(new(), consumerKey, consumerSecret).Should().ThrowExactly<ArgumentNullException>();

        /// <summary>
        /// HttpClientがnullのとき、<see cref="ArgumentNullException"/>の例外をスローする。
        /// </summary>
        [Fact(DisplayName = TestNameConstractor + nameof(ArgumentNullException) + "をスローする。")]
        public void Constractor_Throws_ArgumentNullException_WhenClientIsNull()
            => Constractor(null, "foo", "bar").Should().ThrowExactly<ArgumentNullException>();

        /// <summary>
        /// <see cref="HttpClient.BaseAddress"/>がnullのとき、既定値をセットする。
        /// </summary>
        [Fact(DisplayName = TestNameConstractor + nameof(HttpClient.BaseAddress) + "がnullのとき、既定値をセットする。")]
        public void Constractor_Sets_BaseAddress_WhenIsNull()
        {
            // Arrange
            var client = new HttpClient();
            client.BaseAddress.Should().BeNull();

            // Act
            _ = new KaonaviV2Service(client, "foo", "bar");

            // Assert
            client.BaseAddress.Should().NotBeNull();
        }

        /// <summary>
        /// <see cref="HttpClient.BaseAddress"/>がnullでないときは、既定値をセットしない。
        /// </summary>
        [Fact(DisplayName = TestNameConstractor + nameof(HttpClient.BaseAddress) + "がnullでないときは、既定値をセットしない。")]
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
            client.BaseAddress.Should().Be(_baseUri);
        }
        #endregion

        #region Property
        /// <summary>
        /// <see cref="KaonaviV2Service.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を返す。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.AccessToken) + "(get) > Kaonavi-Tokenヘッダーの値を返す。")]
        public void AccessToken_Returns_ClientHeader()
        {
            // Arrange
            var client = new HttpClient();
            string headerValue = GenerateRandomString();
            client.DefaultRequestHeaders.Add("Kaonavi-Token", headerValue);

            // Act
            var sut = new KaonaviV2Service(client, "foo", "bar");

            // Assert
            sut.AccessToken.Should().Be(headerValue);
        }

        /// <summary>
        /// Kaonavi-Tokenヘッダーがないとき、<see cref="KaonaviV2Service.AccessToken"/>は、nullを返す。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.AccessToken) + "(get) > Kaonavi-Tokenヘッダーがないとき、nullを返す。")]
        public void AccessToken_Returns_Null()
        {
            // Arrange - Act
            var sut = new KaonaviV2Service(new(), "foo", "bar");

            // Assert
            sut.AccessToken.Should().BeNull();
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.AccessToken"/>は、Kaonavi-Tokenヘッダーの値を設定する。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.AccessToken) + "(set) > Kaonavi-Tokenヘッダーの値を設定する。")]
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
            client.DefaultRequestHeaders.TryGetValues("Kaonavi-Token", out var values).Should().BeTrue();
            values.Should().HaveCount(1).And.Contain(headerValue);
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.UseDryRun"/>は、HttpClientのDry-Runヘッダーが"1"かどうかを返す。
        /// </summary>
        /// <param name="headerValue">Dry-Runヘッダーに設定する値</param>
        /// <param name="expected"><see cref="KaonaviV2Service.UseDryRun"/></param>
        [Theory(DisplayName = TestName + nameof(KaonaviV2Service.UseDryRun) + "(get) > Dry-Run: 1 かどうかを返す。")]
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
            sut.UseDryRun.Should().Be(expected);
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.UseDryRun"/>は、HttpClientのDry-Runヘッダー値を追加/削除する。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.UseDryRun) + "(set) > Dry-Runヘッダーを追加/削除する。")]
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
            client.DefaultRequestHeaders.TryGetValues("Dry-Run", out var values).Should().BeTrue();
            values!.First().Should().Be("1");
            #endregion

            #region UseDryRun = false
            // Act
            sut.UseDryRun = false;

            // Assert
            client.DefaultRequestHeaders.TryGetValues("Dry-Run", out _).Should().BeFalse();
            #endregion
        }
        #endregion

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
        [Theory(DisplayName = TestName + "API Caller > " + nameof(ApplicationException) + "をスローする。")]
        [InlineData(401, "{{\"errors\":[\"{0}\"]}}", "consumer_keyとconsumer_secretの組み合わせが不正です。", "application/json")]
        [InlineData(429, "{{\"errors\":[\"{0}\"]}}", "1時間あたりのトークン発行可能数を超過しました。時間をおいてお試しください。", "application/json")]
        [InlineData(500, "{0}", "Error", "plain/text")]
        public async Task ApiCaller_Throws_ApplicationException(int statusCode, string contentFormat, string message, string? mediaType)
        {
            // Arrange
            string key = GenerateRandomString();
            string secret = GenerateRandomString();
            var endpoint = new Uri(_baseUri, "/token");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse((HttpStatusCode)statusCode, string.Format(contentFormat, message), mediaType);

            // Act
            var sut = CreateSut(handler, key, secret);
            Func<Task> act = async () => await sut.AuthenticateAsync().ConfigureAwait(false);

            (await act.Should().ThrowExactlyAsync<ApplicationException>().ConfigureAwait(false))
                .WithMessage(message)
                .WithInnerExceptionExactly<HttpRequestException>();
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.AccessToken"/>がnullのとき、<see cref="KaonaviV2Service.AuthenticateAsync(CancellationToken)"/>を呼び出す。
        /// </summary>
        [Fact(DisplayName = TestName + "API Caller > " + nameof(KaonaviV2Service.AuthenticateAsync) + "を呼び出す。")]
        public async Task ApiCaller_Calls_AuthenticateAsync_When_AccessToken_IsNull()
        {
            var apiEndpoint = new Uri(_baseUri, "/member_layouts");
            var tokenEndpoint = new Uri(_baseUri, "/token");
            string tokenString = GenerateRandomString();
            string key = GenerateRandomString();
            string secret = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == tokenEndpoint)
                .ReturnsResponse(HttpStatusCode.InternalServerError, "Error", "text/plain");

            // Act
            var sut = CreateSut(handler, key, secret);
            Func<Task> act = async () => await sut.FetchMemberLayoutAsync().ConfigureAwait(false);

            // Assert
            await act.Should().ThrowExactlyAsync<ApplicationException>().ConfigureAwait(false);

            byte[] byteArray = Encoding.UTF8.GetBytes($"{key}:{secret}");
            string base64String = Convert.ToBase64String(byteArray);
            handler.VerifyRequest(IsExpectedRequest, Times.Once());
            handler.VerifyRequest(res => res.RequestUri == apiEndpoint, Times.Never());

            async Task<bool> IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == tokenEndpoint
                && req.Method == HttpMethod.Post
                && req.Headers.Authorization!.Scheme == "Basic"
                && req.Headers.Authorization.Parameter == base64String
                && req.Content is FormUrlEncodedContent content
                && await content.ReadAsStringAsync().ConfigureAwait(false) == "grant_type=client_credentials";
        }
        #endregion

        /// <summary>
        /// <see cref="KaonaviV2Service.AuthenticateAsync(CancellationToken)"/>は、"/token"にBase64文字列のPOSTリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.AuthenticateAsync) + " > POST /token をコールする。")]
        public async Task AuthenticateAsync_Posts_Base64String()
        {
            // Arrange
            string key = GenerateRandomString();
            string secret = GenerateRandomString();
            var endpoint = new Uri(_baseUri, "/token");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsJson(new Token(tokenString, "Bearer", 3600));

            // Act
            var sut = CreateSut(handler, key, secret);
            var token = await sut.AuthenticateAsync().ConfigureAwait(false);

            // Assert
            token.Should().NotBeNull();
            token.Should().Be(new Token(tokenString, "Bearer", 3600));

            byte[] byteArray = Encoding.UTF8.GetBytes($"{key}:{secret}");
            string base64String = Convert.ToBase64String(byteArray);
            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            async Task<bool> IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                && req.Method == HttpMethod.Post
                && req.Headers.Authorization!.Scheme == "Basic"
                && req.Headers.Authorization.Parameter == base64String
                && req.Content is FormUrlEncodedContent content
                && await content.ReadAsStringAsync().ConfigureAwait(false) == "grant_type=client_credentials";
        }

        #region Layout API
        /// <summary>
        /// <see cref="KaonaviV2Service.FetchMemberLayoutAsync(CancellationToken)"/>は、"/member_layouts"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchMemberLayoutAsync) + " > GET /member_layouts をコールする。")]
        public async Task FetchMemberLayoutAsync_Returns_MemberLayout()
        {
            var endpoint = new Uri(_baseUri, "/member_layouts");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsJson(new MemberLayout(
                    new("社員番号", true, FieldType.String, 50, Array.Empty<string>()),
                    new("氏名", false, FieldType.String, 100, Array.Empty<string>()),
                    new("フリガナ", false, FieldType.String, 100, Array.Empty<string>()),
                    new("メールアドレス", false, FieldType.String, 100, Array.Empty<string>()),
                    new("入社日", false, FieldType.Date, null, Array.Empty<string>()),
                    new("退職日", false, FieldType.Date, null, Array.Empty<string>()),
                    new("性別", false, FieldType.Enum, null, new[] { "男性", "女性" }),
                    new("生年月日", false, FieldType.Date, null, Array.Empty<string>()),
                    new("所属", false, FieldType.Department, null, Array.Empty<string>()),
                    new("兼務情報", false, FieldType.DepartmentArray, null, Array.Empty<string>()),
                    new CustomField[]
                    {
                        new(100, "血液型", false, FieldType.Enum, null, new[]{ "A", "B", "O", "AB" }),
                        new(200, "役職", false, FieldType.Enum, null, new[]{ "部長", "課長", "マネージャー", null }),
                    }
                ));

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var layout = await sut.FetchMemberLayoutAsync().ConfigureAwait(false);

            // Assert
            layout.Should().NotBeNull();
            layout!.Code.Name.Should().Be("社員番号");
            layout.Name.Required.Should().BeFalse();
            layout.NameKana.Type.Should().Be(FieldType.String);
            layout.Mail.MaxLength.Should().Be(100);
            layout.EnteredDate.Type.Should().Be(FieldType.Date);
            layout.RetiredDate.Enum.Should().BeEmpty();
            layout.Gender.Enum.Should().Equal("男性", "女性");
            layout.Birthday.MaxLength.Should().BeNull();
            layout.Department.Type.Should().Be(FieldType.Department);
            layout.SubDepartments.Type.Should().Be(FieldType.DepartmentArray);
            layout.CustomFields.Should().HaveCount(2);
            layout.CustomFields[^1].Enum.Should().Equal("部長", "課長", "マネージャー", null);

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.FetchSheetLayoutsAsync(CancellationToken)"/>は、"/sheet_layouts"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchSheetLayoutsAsync) + " > GET /sheet_layouts をコールする。")]
        public async Task FetchSheetLayoutsAsync_Returns_SheetLayouts()
        {
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
            var endpoint = new Uri(_baseUri, "/sheet_layouts");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var layouts = await sut.FetchSheetLayoutsAsync().ConfigureAwait(false);

            // Assert
            layouts.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            var layout = layouts[0];
            layout.Id.Should().Be(12);
            layout.Name.Should().Be("住所・連絡先");
            layout.RecordType.Should().Be(RecordType.Multiple);
            layout.CustomFields.Should().HaveCount(2)
                .And.AllBeAssignableTo<CustomField>();

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }
        #endregion

        #region Member API
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
                SubDepartments: Array.Empty<Department>(),
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
                SubDepartments: new Department[]
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
        /// <see cref="KaonaviV2Service.FetchMembersDataAsync(CancellationToken)"/>は、"/members"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchMembersDataAsync) + " > GET /members をコールする。")]
        public async Task FetchMembersDataAsync_Returns_MemberDataList()
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
            #endregion
            var endpoint = new Uri(_baseUri, "/members");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var members = await sut.FetchMembersDataAsync().ConfigureAwait(false);

            // Assert
            members.Should().HaveCount(2);

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.AddMemberDataAsync"/>は、"/members"にPOSTリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.AddMemberDataAsync) + " > POST /members をコールする。")]
        public async Task AddMemberDataAsync_Returns_TaskId()
        {
            // Arrange
            var endpoint = new Uri(_baseUri, "/members");
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, JsonConfig.Default)}}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.AddMemberDataAsync(_memberDataPayload).ConfigureAwait(false);

            // Assert
            taskId.Should().Be(1);

            string? receivedJson = null;
            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Post
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) is not null,
                    Times.Once());
            receivedJson.Should().Be(expectedJson);
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.ReplaceMemberDataAsync"/>は、"/members"にPUTリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.ReplaceMemberDataAsync) + " > PUT /members をコールする。")]
        public async Task ReplaceMemberDataAsync_Returns_TaskId()
        {
            // Arrange
            var endpoint = new Uri(_baseUri, "/members");
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, JsonConfig.Default)}}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.ReplaceMemberDataAsync(_memberDataPayload).ConfigureAwait(false);

            // Assert
            taskId.Should().Be(1);

            string? receivedJson = null;
            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Put
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) is not null,
                    Times.Once());
            receivedJson.Should().Be(expectedJson);
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.UpdateMemberDataAsync"/>は、"/members"にPATCHリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.UpdateMemberDataAsync) + " > PATCH /members をコールする。")]
        public async Task UpdateMemberDataAsync_Returns_TaskId()
        {
            // Arrange
            var endpoint = new Uri(_baseUri, "/members");
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_memberDataPayload, JsonConfig.Default)}}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.UpdateMemberDataAsync(_memberDataPayload).ConfigureAwait(false);

            // Assert
            taskId.Should().Be(1);

            string? receivedJson = null;
            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Patch
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) is not null,
                    Times.Once());
            receivedJson.Should().Be(expectedJson);
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.DeleteMemberDataAsync"/>は、"/members/delete"にPOSTリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.DeleteMemberDataAsync) + " > POST /members/delete をコールする。")]
        public async Task DeleteMemberDataAsync_Returns_TaskId()
        {
            // Arrange
            var endpoint = new Uri(_baseUri, "/members/delete");
            string[] codes = _memberDataPayload.Select(d => d.Code).ToArray();
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"codes\":{JsonSerializer.Serialize(codes, JsonConfig.Default)}}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.DeleteMemberDataAsync(codes).ConfigureAwait(false);

            // Assert
            taskId.Should().Be(1);

            string? receivedJson = null;
            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Post
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) is not null,
                    Times.Once());
            receivedJson.Should().Be(expectedJson);
        }
        #endregion

        #region Sheet API
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
        /// <see cref="KaonaviV2Service.FetchSheetDataListAsync(int, CancellationToken)"/>は、"/sheets/{sheetId}"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchSheetDataListAsync) + " > GET /sheets/{sheetId} をコールする。")]
        public async Task FetchSheetDataListAsync_Returns_SheetDataList()
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
            #endregion
            var endpoint = new Uri(_baseUri, $"/sheets/{sheetId}");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var members = await sut.FetchSheetDataListAsync(sheetId).ConfigureAwait(false);

            // Assert
            members.Should().HaveCount(2);

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.ReplaceSheetDataAsync"/>は、"/sheets/{sheetId}"にPUTリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.ReplaceSheetDataAsync) + " > PUT /sheets/{sheetId} をコールする。")]
        public async Task ReplaceSheetDataAsync_Returns_TaskId()
        {
            // Arrange
            const int sheetId = 1;
            var endpoint = new Uri(_baseUri, $"/sheets/{sheetId}");
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_sheetDataPayload, JsonConfig.Default)}}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.ReplaceSheetDataAsync(sheetId, _sheetDataPayload).ConfigureAwait(false);

            // Assert
            taskId.Should().Be(sheetId);

            string? receivedJson = null;
            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Put
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) is not null,
                    Times.Once());
            receivedJson.Should().Be(expectedJson);
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.UpdateSheetDataAsync"/>は、"/sheets/{sheetId}"にPATCHリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.UpdateSheetDataAsync) + " > PATCH /sheets/{sheetId} をコールする。")]
        public async Task UpdateSheetDataAsync_Returns_TaskId()
        {
            // Arrange
            const int sheetId = 1;
            var endpoint = new Uri(_baseUri, $"/sheets/{sheetId}");
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"member_data\":{JsonSerializer.Serialize(_sheetDataPayload, JsonConfig.Default)}}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.UpdateSheetDataAsync(sheetId, _sheetDataPayload).ConfigureAwait(false);

            // Assert
            taskId.Should().Be(sheetId);

            string? receivedJson = null;
            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Patch
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) is not null,
                    Times.Once());
            receivedJson.Should().Be(expectedJson);
        }
        #endregion

        #region Department API
        /// <summary>
        /// <see cref="KaonaviV2Service.FetchDepartmentsAsync(CancellationToken)"/>は、"/departments"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchDepartmentsAsync) + " > GET /departments をコールする。")]
        public async Task FetchDepartmentsAsync_Returns_DepartmentInfoList()
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
            #endregion
            var endpoint = new Uri(_baseUri, "/departments");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var departments = await sut.FetchDepartmentsAsync().ConfigureAwait(false);

            // Assert
            departments.Should().Equal(
                new("1000", "取締役会", null, "A0002", 1, ""),
                new("1200", "営業本部", null, null, 2, ""),
                new("1500", "第一営業部", "1200", null, 1, ""),
                new("2000", "ITグループ", "1500", "A0001", 1, "example")
            );

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.ReplaceDepartmentsAsync"/>は、"/departments"にPUTリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.ReplaceDepartmentsAsync) + " > PUT /departments をコールする。")]
        public async Task ReplaceDepartmentsAsync_Returns_TaskId()
        {
            // Arrange
            const int sheetId = 1;
            var payload = new DepartmentInfo[]
            {
                new("1000", "取締役会", null, "A0002", 1, ""),
                new("1200", "営業本部", null, null, 2, ""),
                new("1500", "第一営業部", "1200", null, 1, ""),
                new("2000", "ITグループ", "1500", "A0001", 1, "example"),
            };
            var endpoint = new Uri(_baseUri, "/departments");
            string tokenString = GenerateRandomString();
            string expectedJson = $"{{\"department_data\":{JsonSerializer.Serialize(payload, JsonConfig.Default)}}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, TaskJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            int taskId = await sut.ReplaceDepartmentsAsync(payload).ConfigureAwait(false);

            // Assert
            taskId.Should().Be(sheetId);

            string? receivedJson = null;
            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Put
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (receivedJson = await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) is not null,
                    Times.Once());
            receivedJson.Should().Be(expectedJson);
        }
        #endregion

        #region Task API
        private const string TestNameFetchTaskProgressAsync = TestName + nameof(KaonaviV2Service.FetchTaskProgressAsync) + " > ";

        /// <summary>
        /// taskIdが<c>0</c>未満のとき、<see cref="KaonaviV2Service.FetchTaskProgressAsync(int, CancellationToken)"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [Fact(DisplayName = TestNameFetchTaskProgressAsync + nameof(ArgumentOutOfRangeException) + "をスローする。")]
        public async Task FetchTaskProgressAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            Func<Task> act = async () => _ = await sut.FetchTaskProgressAsync(-1).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithMessage("*taskId*")
                .ConfigureAwait(false);

            handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.FetchTaskProgressAsync(int, CancellationToken)"/>は、"/tasks/{taskId}"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestNameFetchTaskProgressAsync + "GET /tasks/{taskId} をコールする。")]
        public async Task FetchTaskProgressAsync_Returns_TaskProgress()
        {
            const int taskId = 1;
            var endpoint = new Uri(_baseUri, $"/tasks/{taskId}");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsJson(new TaskProgress(taskId, "NG", new[] { "エラーメッセージ1", "エラーメッセージ2" }));

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var task = await sut.FetchTaskProgressAsync(taskId).ConfigureAwait(false);

            // Assert
            task.Should().NotBeNull();
            task.Id.Should().Be(taskId);
            task.Status.Should().Be("NG");
            task.Messages.Should().Equal("エラーメッセージ1", "エラーメッセージ2");

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }
        #endregion

        #region User API
        /// <summary>
        /// <see cref="KaonaviV2Service.FetchUsersAsync(CancellationToken)"/>は、"/users"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchUsersAsync) + " > GET /users をコールする。")]
        public async Task FetchUsersAsync_Returns_Users()
        {
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
            + "    }"
            + "  },"
            + "  {"
            + "    \"id\": 2,"
            + "    \"email\": \"hanako@kaonavi.jp\","
            + "    \"member_code\": \"A0001\","
            + "    \"role\": {"
            + "      \"id\": 2,"
            + "      \"name\": \"マネージャ\","
            + "      \"type\": \"一般\""
            + "    }"
            + "  }"
            + "]"
            + "}";
            #endregion
            var endpoint = new Uri(_baseUri, "/users");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var users = await sut.FetchUsersAsync().ConfigureAwait(false);

            // Assert
            users.Should().Equal(
                new User(1, "taro@kaonavi.jp", "A0002", new(1, "システム管理者", "Adm")),
                new User(2, "hanako@kaonavi.jp", "A0001", new(2, "マネージャ", "一般"))
            );

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.AddUserAsync(UserPayload, CancellationToken)"/>は、"/users/{userId}"にPATCHリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.AddUserAsync) + " > POST /users をコールする。")]
        public async Task AddUserAsync_Returns_User()
        {
            var endpoint = new Uri(_baseUri, "/users");
            string tokenString = GenerateRandomString();
            var payload = new UserPayload("user1@example.com", "00001", "password", 1);
            const string expectedJson = "{\"email\":\"user1@example.com\","
            + "\"member_code\":\"00001\","
            + "\"password\":\"password\","
            + "\"role\":{\"id\":1}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsJson(new User(10, "user1@example.com", "00001", new(1, "Admin", "Adm")));

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var user = await sut.AddUserAsync(payload).ConfigureAwait(false);

            // Assert
            user.Should().Be(new User(10, "user1@example.com", "00001", new(1, "Admin", "Adm")));

            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Post
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) == expectedJson,
                    Times.Once());
        }

        /// <summary>
        /// userIdが<c>0</c>未満のとき、<see cref="KaonaviV2Service.FetchUserAsync(int, CancellationToken)"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchUsersAsync) + " > " + nameof(ArgumentOutOfRangeException) + "をスローする。")]
        public async Task FetchUserAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            Func<Task> act = async () => _ = await sut.FetchUserAsync(-1).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithMessage("*userId*")
                .ConfigureAwait(false);

            handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.FetchUserAsync(int, CancellationToken)"/>は、"/users/{userId}"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchUserAsync) + " > GET /users/{userId} をコールする。")]
        public async Task FetchUserAsync_Returns_User()
        {
            const int userId = 1;
            var endpoint = new Uri(_baseUri, $"/users/{userId}");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsJson(new User(userId, "user1@example.com", "00001", new(1, "Admin", "Adm")));

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var user = await sut.FetchUserAsync(userId).ConfigureAwait(false);

            // Assert
            user.Should().Be(new User(userId, "user1@example.com", "00001", new(1, "Admin", "Adm")));

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }

        /// <summary>
        /// userIdが<c>0</c>未満のとき、<see cref="KaonaviV2Service.UpdateUserAsync(int, UserPayload, CancellationToken)"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.UpdateUserAsync) + " > " + nameof(ArgumentOutOfRangeException) + "をスローする。")]
        public async Task UpdateUserAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            Func<Task> act = async () => _ = await sut.UpdateUserAsync(-1, null!).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithMessage("*userId*")
                .ConfigureAwait(false);

            handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.UpdateUserAsync(int, UserPayload, CancellationToken)"/>は、"/users/{userId}"にPATCHリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.UpdateUserAsync) + " > PATCH /users/{userId} をコールする。")]
        public async Task UpdateUserAsync_Returns_User()
        {
            const int userId = 1;
            var endpoint = new Uri(_baseUri, $"/users/{userId}");
            string tokenString = GenerateRandomString();
            var payload = new UserPayload("user1@example.com", "00001", "password", 1);
            const string expectedJson = "{\"email\":\"user1@example.com\","
            + "\"member_code\":\"00001\","
            + "\"password\":\"password\","
            + "\"role\":{\"id\":1}}";

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsJson(new User(userId, "user1@example.com", "00001", new(1, "Admin", "Adm")));

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var user = await sut.UpdateUserAsync(userId, payload).ConfigureAwait(false);

            // Assert
            user.Should().Be(new User(userId, "user1@example.com", "00001", new(1, "Admin", "Adm")));

            handler.VerifyRequest(async (req) => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Patch
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString
                    && (await req.Content!.ReadAsStringAsync().ConfigureAwait(false)) == expectedJson,
                    Times.Once());
        }

        /// <summary>
        /// userIdが<c>0</c>未満のとき、<see cref="KaonaviV2Service.DeleteUserAsync(int, CancellationToken)"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.DeleteUserAsync) + " > " + nameof(ArgumentOutOfRangeException) + "をスローする。")]
        public async Task DeleteUserAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(It.IsAny<Uri>()).ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            Func<Task> act = async () => await sut.DeleteUserAsync(-1).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithMessage("*userId*")
                .ConfigureAwait(false);

            handler.VerifyRequest(It.IsAny<Uri>(), Times.Never());
        }

        /// <summary>
        /// <see cref="KaonaviV2Service.DeleteUserAsync(int, CancellationToken)"/>は、"/users/{userId}"にDELETEリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.DeleteUserAsync) + " > DELETE /users/{userId} をコールする。")]
        public async Task DeleteUserAsync_Returns_User()
        {
            const int userId = 1;
            var endpoint = new Uri(_baseUri, $"/users/{userId}");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.NoContent);

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            await sut.DeleteUserAsync(userId).ConfigureAwait(false);

            // Assert
            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Delete
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }
        #endregion

        /// <summary>
        /// <see cref="KaonaviV2Service.FetchRolesAsync"/>は、"/roles"にGETリクエストを行う。
        /// </summary>
        [Fact(DisplayName = TestName + nameof(KaonaviV2Service.FetchRolesAsync) + " > GET /roles をコールする。")]
        public async Task FetchRolesAsync_Returns_Roles()
        {
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
            var endpoint = new Uri(_baseUri, "/roles");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var roles = await sut.FetchRolesAsync().ConfigureAwait(false);

            // Assert
            roles.Should().Equal(
                new Role(1, "カオナビ管理者", "Adm"),
                new Role(2, "カオナビマネージャー", "一般"));

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }
    }
}
