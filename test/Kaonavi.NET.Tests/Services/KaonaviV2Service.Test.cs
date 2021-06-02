using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kaonavi.Net.Entities;
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

        [Theory]
        [InlineData(401, "{{\"errors\":[\"{0}\"]}}", "consumer_keyとconsumer_secretの組み合わせが不正です。", "application/json")]
        [InlineData(429, "{{\"errors\":[\"{0}\"]}}", "1時間あたりのトークン発行可能数を超過しました。時間をおいてお試しください。", "application/json")]
        [InlineData(500, "{0}", "Error", "plain/text")]
        public async Task AuthenticateAsync_Throws_ApplicationException(int statusCode, string contentFormat, string message, string mediaType)
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
                .Where(ex => ex.Message == message && ex.InnerException is HttpRequestException);
        }
    }
}
