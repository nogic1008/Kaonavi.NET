using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="Attachment"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class AttachmentTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [TestMethod(DisplayName = $"{nameof(Attachment)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
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
        var attachment = JsonSerializer.Deserialize(json, Context.Default.Attachment);

        // Assert
        attachment.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Code.ShouldBe("A0001"),
            static sut => sut.Records.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
                static sut => sut.FileName.ShouldBe("sample.txt"),
                static sut => sut.Content.ShouldBe(Convert.FromBase64String("44GT44KM44Gv44K144Oz44OX44Or44OG44Kt44K544OI44Gn44GZ44CC"))
            )
        );
    }
}
