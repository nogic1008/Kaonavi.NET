using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="Attachment"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class AttachmentTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [TestMethod($"{nameof(Attachment)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeJSON()
    {
        // Arrange
        // lang=json,strict
        const string json = """
        {
          "code": "A0001",
          "records": [
            {
              "file_name": "sample.txt",
              "base64_content": "44GT44KM44Gv44K144Oz44OX44Or44OG44Kt44K544OI44Gn44GZ44CC"
            }
          ]
        }
        """;

        // Act
        var sut = JsonSerializer.Deserialize(json, Context.Default.Attachment);

        // Assert
        _ = sut.Should().NotBeNull();
        _ = sut!.Code.Should().Be("A0001");
        _ = sut.Records.Should().HaveCount(1).And.ContainSingle(r => r.FileName == "sample.txt"
            && r.Content.SequenceEqual(Convert.FromBase64String("44GT44KM44Gv44K144Oz44OX44Or44OG44Kt44K544OI44Gn44GZ44CC")));
    }
}
