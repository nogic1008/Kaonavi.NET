using System;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Kaonavi.Net.Services;
using Xunit;

namespace Kaonavi.Net.Tests.Services
{
    /// <summary>
    /// Unit test for <see cref="KaonaviV2Service"/>
    /// </summary>
    public class KaonaviV2ServiceTest
    {
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
            string headerValue = Guid.NewGuid().ToString();
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
            string headerValue = Guid.NewGuid().ToString();

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
    }
}
