using FluentAssertions.Primitives;

namespace Kaonavi.Net.Tests.Assertions;

/// <summary>
/// Contains assertions for <see cref="HttpRequestMessage"/> instances.
/// </summary>
public class HttpRequestMessageAssertions(HttpRequestMessage instance) :
    ReferenceTypeAssertions<HttpRequestMessage, HttpRequestMessageAssertions>(instance)
{
    protected override string Identifier => nameof(HttpRequestMessage);

    /// <summary>
    /// Asserts that the HttpRequestMessage has the specified method and path.
    /// </summary>
    public AndConstraint<HttpRequestMessageAssertions> SendTo(
        HttpMethod method, string pathAndQuery, string because = "", params object[] becauseArgs)
    {
        Subject.Method.Should().Be(method, because, becauseArgs);
        Subject.RequestUri.Should().NotBeNull(because, becauseArgs);
        Subject.RequestUri?.PathAndQuery.Should().Be(pathAndQuery, because, becauseArgs);
        return new AndConstraint<HttpRequestMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HttpRequestMessage has a Kaonavi-Token header with the <paramref name="expected"/> value.
    /// </summary>
    public AndConstraint<HttpRequestMessageAssertions> HasToken(string expected, string because = "", params object[] becauseArgs)
    {
        Subject.Headers.GetValues("Kaonavi-Token").Should()
            .ContainSingle(because, becauseArgs)
            .And.Contain(expected, because, becauseArgs);
        return new AndConstraint<HttpRequestMessageAssertions>(this);
    }
}
