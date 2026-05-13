using TUnit.Assertions.Attributes;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Kaonavi.Net.Tests.Assertions;

[AssertionExtension("IsSequenceEqualTo")]
public class CollectionIsSequenceEqualAssertion<TCollection, TItem>(
    AssertionContext<TCollection> context,
    IEnumerable<TItem> expected
) : CollectionAssertionBase<TCollection, TItem>(context)
    where TCollection : IEnumerable<TItem>
{
    private readonly IEnumerable<TItem> _expected =
        expected ?? throw new ArgumentNullException(nameof(expected));

    /// <inheritdoc />
    protected override Task<AssertionResult> CheckAsync(EvaluationMetadata<TCollection> metadata)
    {
        var value = metadata.Value;
        var exception = metadata.Exception;

        if (exception is not null)
            return Task.FromResult(AssertionResult.Failed($"threw {exception.GetType().Name}"));

        if (value is null)
            return Task.FromResult(AssertionResult.Failed("collection was null"));
        if (value.SequenceEqual(_expected))
            return Task.FromResult(AssertionResult.Passed);

        return Task.FromResult(
            AssertionResult.Failed(
                $"'{string.Join(", ", value)}' does not equal '{string.Join(", ", _expected)}'"
            )
        );
    }
}
