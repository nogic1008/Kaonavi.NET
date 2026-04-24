using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TUnit.Assertions.Core;

namespace Kaonavi.Net.Tests.Assertions;

internal class JsonElementAssertion : Assertion<JsonElement>
{
    private readonly JsonElement _expected;
    public JsonElementAssertion(AssertionContext<JsonElement> context, JsonElement expected) : base(context)
        => _expected = expected;
    public JsonElementAssertion(AssertionContext<JsonElement> context, [StringSyntax(StringSyntaxAttribute.Json)] string expected)
        : this(context, JsonElement.Parse(expected)) { }
    public JsonElementAssertion(AssertionContext<JsonElement> context, ReadOnlySpan<byte> expected)
        : this(context, JsonElement.Parse(expected)) { }

    public JsonElementAssertion(AssertionContext<string> context, JsonElement expected)
        : base(context.Map(static json => JsonDocument.Parse(json!).RootElement)) => _expected = expected;
    public JsonElementAssertion(AssertionContext<string> context, [StringSyntax(StringSyntaxAttribute.Json)] string expected)
        : this(context, JsonElement.Parse(expected)) { }
    public JsonElementAssertion(AssertionContext<string> context, ReadOnlySpan<byte> expected)
        : this(context, JsonElement.Parse(expected)) { }

    protected override Task<AssertionResult> CheckAsync(EvaluationMetadata<JsonElement> metadata)

    {
        var val = metadata.Value;
        var exception = metadata.Exception;

        if (exception is not null)
            return Task.FromResult(AssertionResult.Failed($"threw {exception.GetType().Name}"));
        if (JsonElement.DeepEquals(val, _expected))
            return Task.FromResult(AssertionResult.Passed);
        else
            return Task.FromResult(AssertionResult.Failed($"'{val.GetRawText()}' does not equal '{_expected.GetRawText()}'"));
    }

    protected override string GetExpectation() => $"to equal \"{_expected.GetRawText()}\"";
}

internal static class JsonElementAssertionExtensions
{
    extension(IAssertionSource<JsonElement> source)
    {
        public JsonElementAssertion IsJsonEquals([StringSyntax(StringSyntaxAttribute.Json)] string expected)
            => new(source.Context, expected);
        public JsonElementAssertion IsJsonEquals(JsonElement expected)
            => new(source.Context, expected);
        public JsonElementAssertion IsJsonEquals(ReadOnlySpan<byte> expected)
            => new(source.Context, expected);
    }

    extension(IAssertionSource<string> source)
    {
        public JsonElementAssertion IsJsonEquals([StringSyntax(StringSyntaxAttribute.Json)] string expected)
            => new(source.Context, expected);
        public JsonElementAssertion IsJsonEquals(JsonElement expected)
            => new(source.Context, expected);
        public JsonElementAssertion IsJsonEquals(ReadOnlySpan<byte> expected)
            => new(source.Context, expected);
    }
}
