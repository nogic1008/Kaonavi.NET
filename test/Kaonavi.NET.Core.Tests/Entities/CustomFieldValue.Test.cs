using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="CustomFieldValue"/>の単体テスト</summary>
[Category("Entities")]
public sealed class CustomFieldValueTest
{
    private const string TestName = $"{nameof(CustomFieldValue)} > JSONからデシリアライズできる。";

    /// <summary>JSONからデシリアライズできる。</summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="CustomFieldValue.Id" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="CustomFieldValue.Name" path="/summary"/></param>
    /// <param name="values"><inheritdoc cref="CustomFieldValue.Values" path="/summary"/></param>
    [Test(TestName)]
    [Category("JSON Deserialize")]
    [Arguments("""{ "id": 100, "name": "血液型", "values": ["A"] }""", 100, "血液型", "A", DisplayName = TestName)]
    [Arguments("""{ "id": 100, "values": [""] }""", 100, null, "", DisplayName = TestName)]
    [Arguments("""{ "id": 1, "values": ["Aコース", "Bコース"] }""", 1, null, "Aコース", "Bコース", DisplayName = TestName)]
    public async Task CanDeserializeJSON([StringSyntax(StringSyntaxAttribute.Json)] string json, int id, string? name, params string[] values)
    {
        // Arrange - Act
        var customFieldValue = JsonSerializer.Deserialize(json, JsonContext.Default.CustomFieldValue);

        // Assert
        await Assert.That(customFieldValue).IsNotNull()
            .And.Member(static sut => sut.Id, o => o.IsEqualTo(id))
            .And.Member(static sut => sut.Name!, o => name is null ? o.IsNull() : o.IsEqualTo<string>(name))
            .And.Member(static sut => sut.Value, o => values.Length == 0 ? o.IsEmpty() : o.IsEqualTo<string>(values[0]))
            .And.Member(static sut => sut.Values, o => o.IsEquivalentTo(values));
    }
}
