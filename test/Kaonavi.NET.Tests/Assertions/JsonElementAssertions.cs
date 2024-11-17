using System.Diagnostics.CodeAnalysis;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Kaonavi.Net.Tests.Assertions;

/// <summary>
/// Contains assertions for <see cref="JsonElement"/> instances.
/// </summary>
public class JsonElementAssertions(JsonElement instance) :
    ReferenceTypeAssertions<JsonElement, JsonElementAssertions>(instance)
{
    protected override string Identifier => nameof(JsonElement);

    /// <summary>
    /// Asserts that the JsonElement has same layout.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<JsonElementAssertions> BeSameJson([StringSyntax(StringSyntaxAttribute.Json)] string expected, string because = "", params object[] becauseArgs)
    {
        using var doc = JsonDocument.Parse(expected);
        Execute.Assertion
            .ForCondition(JsonElement.DeepEquals(Subject, doc.RootElement))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:JsonElement} to be the same as {0}{reason}, but found {1}.", expected, Subject);
        return new AndConstraint<JsonElementAssertions>(this);
    }
}
