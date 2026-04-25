using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="AttachmentInfo"/>の単体テスト</summary>
[Category("Entities")]
public sealed class AttachmentInfoTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [Test($"{nameof(AttachmentInfo)} > JSONからデシリアライズできる。"), Category("JSON Deserialize")]
    public async Task CanDeserializeJSON()
    {
        // Arrange
        // lang=json,strict
        var json = """
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
        """u8;

        // Act
        var attachment = JsonSerializer.Deserialize(json, JsonContext.Default.AttachmentInfo);

        // Assert
        await Assert.That(attachment).IsNotNull()
            .And.Member(static o => o.Code, static o => o.IsEqualTo<string>("A0001"))
            .And.Member(static o => o.Records, static o => o.IsEquivalentTo((AttachmentInfoRecord[])[
                new(
                    "A0001.jpg",
                    new("https://example.com/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID"),
                    DateTime.Parse("2020-10-01 01:23:45")
                ),
                new(
                    "A0001.txt",
                    new("https://example.com/image/xxxx.txt?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID"),
                    DateTime.Parse("2020-10-01 01:23:45")
                )
            ]));
    }
}
