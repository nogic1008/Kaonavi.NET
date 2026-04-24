using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FaceImageInfo"/>の単体テスト</summary>
[Category("Entities")]
public sealed class FaceImageInfoTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [Test($"{nameof(FaceImageInfo)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CanDeserializeJSON()
    {
        // Arrange
        // lang=json,strict
        var json = """
        {
          "code": "A0001",
          "file_name": "A0001.jpg",
          "download_url": "https://example.kaonavi.jp/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID",
          "updated_at": "2020-10-01 01:23:45"
        }
        """u8;

        // Act
        var faceImageInfo = JsonSerializer.Deserialize(json, JsonContext.Default.FaceImageInfo);

        // Assert
        await Assert.That(faceImageInfo).IsNotNull()
            .And.Member(static o => o.Code, static o => o.IsEqualTo<string>("A0001"))
            .And.Member(static o => o.FileName, static o => o.IsEqualTo<string>("A0001.jpg"))
            .And.Member(static o => o.DownloadUrl, static o => o.IsEqualTo(new("https://example.kaonavi.jp/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID")))
            .And.Member(static o => o.UpdatedAt, static o => o.IsEqualTo(new(2020, 10, 1, 1, 23, 45)));
    }
}
