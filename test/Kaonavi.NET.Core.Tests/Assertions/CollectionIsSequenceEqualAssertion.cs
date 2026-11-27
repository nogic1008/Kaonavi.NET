using System.ComponentModel;
using TUnit.Assertions.Attributes;

namespace Kaonavi.Net.Tests.Assertions;

internal static partial class CollectionIsSequenceEqualAssertionExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [GenerateAssertion(ExpectationMessage = "to be sequence-equal to {expected}")]
    public static bool IsSequenceEqualTo<TCollection, TItem>(
        this TCollection value,
        IEnumerable<TItem> expected
    )
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(expected);
        return value is not null && value.SequenceEqual(expected);
    }
}
