using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
    /// Unit test for <see cref="KaonaviV2Service"/>
    /// </summary>
    public class KaonaviV2ServiceTest
    {
        private const string BaseUri = "https://example.com";
        private const string TestName = nameof(KaonaviV2Service) + " > ";

        private static string GenerateRandomString() => Guid.NewGuid().ToString();

        #region Constractor
        private static Action Constractor(HttpClient? client, string? consumerKey, string? consumerSecret)
            => () => _ = new KaonaviV2Service(client!, consumerKey!, consumerSecret!);

        [Theory]
        [InlineData(null, "foo")]
        [InlineData("foo", null)]
        public void Constractor_Throws_ArgumentNullException_WhenKeyIsNull(string? consumerKey, string? consumerSecret)
            => Constractor(new(), consumerKey, consumerSecret).Should().ThrowExactly<ArgumentNullException>();

        [Fact]
        public void Constractor_Throws_ArgumentNullException_WhenClientIsNull()
            => Constractor(null, "foo", "bar").Should().ThrowExactly<ArgumentNullException>();

        [Fact]
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

        [Fact]
        public void Constractor_DoesNotSet_BaseAddress_WhenNotNull()
        {
            // Arrange
            var uri = new Uri("https://example.com/");
            var client = new HttpClient
            {
                BaseAddress = uri
            };

            // Act
            _ = new KaonaviV2Service(client, "foo", "bar");

            // Assert
            client.BaseAddress.Should().Be(uri);
        }
        #endregion

        #region Property
        [Fact]
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

        [Fact]
        public void AccessToken_Returns_Null()
        {
            // Arrange - Act
            var sut = new KaonaviV2Service(new(), "foo", "bar");

            // Assert
            sut.AccessToken.Should().BeNull();
        }

        [Fact]
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
            values!.First().Should().Be(headerValue);
        }
        #endregion

        private static KaonaviV2Service CreateSut(Mock<HttpMessageHandler> handler, string key = "Key", string secret = "Secret", string? accessToken = null)
        {
            var client = handler.CreateClient();
            client.BaseAddress = new(BaseUri);
            return new(client, key, secret)
            {
                AccessToken = accessToken
            };
        }

        #region API Common Path
        [Theory]
        [InlineData(401, "{{\"errors\":[\"{0}\"]}}", "consumer_keyとconsumer_secretの組み合わせが不正です。", "application/json")]
        [InlineData(429, "{{\"errors\":[\"{0}\"]}}", "1時間あたりのトークン発行可能数を超過しました。時間をおいてお試しください。", "application/json")]
        [InlineData(500, "{0}", "Error", "plain/text")]
        public async Task ApiCaller_Throws_ApplicationException(int statusCode, string contentFormat, string message, string mediaType)
        {
            // Arrange
            string key = GenerateRandomString();
            string secret = GenerateRandomString();
            var endpoint = new Uri(BaseUri + "/token");
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

        [Fact]
        public async Task ApiCaller_Calls_AuthenticateAsync_When_AccessToken_IsNull()
        {
            var apiEndpoint = new Uri(BaseUri + "/member_layouts");
            var tokenEndpoint = new Uri(BaseUri + "/token");
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

        [Fact]
        public async Task AuthenticateAsync_Posts_Base64String()
        {
            // Arrange
            string key = GenerateRandomString();
            string secret = GenerateRandomString();
            var endpoint = new Uri(BaseUri + "/token");
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

        [Fact]
        public async Task FetchMemberLayoutAsync_Returns_MemberLayout()
        {
            var endpoint = new Uri(BaseUri + "/member_layouts");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsJson(new MemberLayout(
                    Code: new("社員番号", true, "string", 50, Array.Empty<string>()),
                    Name: new("氏名", false, "string", 100, Array.Empty<string>()),
                    NameKana: new("フリガナ", false, "string", 100, Array.Empty<string>()),
                    Mail: new("メールアドレス", false, "string", 100, Array.Empty<string>()),
                    EnteredDate: new("入社日", false, "date", null, Array.Empty<string>()),
                    RetiredDate: new("退職日", false, "date", null, Array.Empty<string>()),
                    Gender: new("性別", false, "enum", null, new[] { "男性", "女性" }),
                    Birthday: new("生年月日", false, "date", null, Array.Empty<string>()),
                    Department: new("所属", false, "department", null, Array.Empty<string>()),
                    SubDepartments: new("兼務情報", false, "department[]", null, Array.Empty<string>()),
                    CustomFields: new CustomField[]
                    {
                        new(100, "血液型", false, "enum", null, new[]{ "A", "B", "O", "AB" }),
                        new(200, "役職", false, "enum", null, new[]{ "部長", "課長", "マネージャー", null }),
                    }
                ));

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var layout = await sut.FetchMemberLayoutAsync().ConfigureAwait(false);

            // Assert
            layout.Should().NotBeNull();
            layout!.Code.Name.Should().Be("社員番号");
            layout.Name.Required.Should().BeFalse();
            layout.NameKana.Type.Should().Be("string");
            layout.Mail.MaxLength.Should().Be(100);
            layout.EnteredDate.Type.Should().Be("date");
            layout.RetiredDate.Enum.Should().BeEmpty();
            layout.Gender.Enum.Should().Equal("男性", "女性");
            layout.Birthday.MaxLength.Should().BeNull();
            layout.Department.Type.Should().Be("department");
            layout.SubDepartments.Type.Should().Be("department[]");
            layout.CustomFields.Should().HaveCount(2);
            layout.CustomFields.Last().Enum.Should().Equal("部長", "課長", "マネージャー", null);

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }

        [Fact]
        public async Task FetchSheetLayoutsAsync_Returns_SheetLayouts()
        {
            var endpoint = new Uri(BaseUri + "/sheet_layouts");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsJson(new KaonaviV2Service.SheetLayoutsResult(
                    new SheetLayout[]
                    {
                        new SheetLayout(
                            Id: 12,
                            Name: "住所・連絡先",
                            RecordType: RecordType.Multiple,
                            CustomFields: new CustomField[]
                            {
                                new(1000, "住所", false, "string", 250, Array.Empty<string?>()),
                                new(1001, "電話番号", false, "string", 50, Array.Empty<string?>())
                            }
                        )
                    }
                ));

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var layouts = await sut.FetchSheetLayoutsAsync().ConfigureAwait(false);

            // Assert
            layouts.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            var layout = layouts.First();
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

        /// <summary>
        /// <see cref="KaonaviV2Service.FetchRolesAsync(System.Threading.CancellationToken)"/>は、"/roles"にGETリクエストを行う。
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
            var endpoint = new Uri(BaseUri + "/roles");
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(req => req.RequestUri == endpoint)
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var roles = await sut.FetchRolesAsync().ConfigureAwait(false);

            // Assert
            roles.Should().Equal(new(1, "カオナビ管理者", "Adm"), new(2, "カオナビマネージャー", "一般"));

            handler.VerifyRequest(IsExpectedRequest, Times.Once());

            bool IsExpectedRequest(HttpRequestMessage req)
                => req.RequestUri == endpoint
                    && req.Method == HttpMethod.Get
                    && req.Headers.TryGetValues("Kaonavi-Token", out var values)
                    && values.First() == tokenString;
        }
    }
}
