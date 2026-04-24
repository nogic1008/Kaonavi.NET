using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="AttachmentInfo"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class AttachmentInfoTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [TestMethod(DisplayName = $"{nameof(AttachmentInfo)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeJSON()
    {
        // Arrange
        // lang=json,strict
        const string json = """
        {
          "code": "A0001",
          "records": [
            {
              "file_name": "A0001.jpg",
              "download_url": "https://example.com/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID",
              "updated_at": "2020-10-01 01:23:45"
            },
            {
              "file_name": "A0001.txt",
              "download_url": "https://example.com/image/xxxx.txt?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID",
              "updated_at": "2020-10-01 01:23:45"
            }
          ]
        }
        """;

        // Act
        var attachment = JsonSerializer.Deserialize(json, Context.Default.AttachmentInfo);

        // Assert
        attachment.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Code.ShouldBe("A0001"),
            static sut => sut.Records.ShouldContain(new AttachmentInfoRecord(
                    "A0001.jpg",
                    new("https://example.com/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID"),
                    DateTime.Parse("2020-10-01 01:23:45")
                )
            ),
            static sut => sut.Records.ShouldContain(new AttachmentInfoRecord(
                    "A0001.txt",
                    new("https://example.com/image/xxxx.txt?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID"),
                    DateTime.Parse("2020-10-01 01:23:45")
                )
            )
        );
    }
}
