using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="CustomFieldValue"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class CustomFieldValueTest
{
    private const string TestName = $"{nameof(CustomFieldValue)} > JSONからデシリアライズできる。";

    /// <summary>JSONからデシリアライズできる。</summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="id"><inheritdoc cref="CustomFieldValue.Id" path="/summary"/></param>
    /// <param name="name"><inheritdoc cref="CustomFieldValue.Name" path="/summary"/></param>
    /// <param name="values"><inheritdoc cref="CustomFieldValue.Values" path="/summary"/></param>
    [TestMethod(TestName), TestCategory("JSON Deserialize")]
    [DataRow(/*lang=json,strict*/ """{ "id": 100, "name": "血液型", "values": ["A"] }""", 100, "血液型", "A", DisplayName = TestName)]
    [DataRow(/*lang=json,strict*/ """{ "id": 100, "values": [""] }""", 100, null, "", DisplayName = TestName)]
    [DataRow(/*lang=json,strict*/ """{ "id": 1, "values": ["Aコース", "Bコース"] }""", 1, null, "Aコース", "Bコース", DisplayName = TestName)]
    public void CanDeserializeJSON(string json, int id, string? name, params string[] values)
    {
        // Arrange - Act
        var customFieldValue = JsonSerializer.Deserialize(json, Context.Default.CustomFieldValue);

        // Assert
        customFieldValue.ShouldNotBeNull().ShouldSatisfyAllConditions(
            sut => sut.Id.ShouldBe(id),
            sut => sut.Name.ShouldBe(name),
            sut => sut.Value.ShouldBe(values[0]),
            sut => sut.Values.ShouldBe(values)
        );
    }
}
