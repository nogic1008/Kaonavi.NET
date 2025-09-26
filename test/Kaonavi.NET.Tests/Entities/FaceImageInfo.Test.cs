using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FaceImageInfo"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class FaceImageInfoTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [TestMethod($"{nameof(FaceImageInfo)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeJSON()
    {
        // Arrange
        // lang=json,strict
        const string json = """
        {
          "code": "A0001",
          "file_name": "A0001.jpg",
          "download_url": "https://example.kaonavi.jp/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID",
          "updated_at": "2020-10-01 01:23:45"
        }
        """;

        // Act
        var faceImageInfo = JsonSerializer.Deserialize(json, Context.Default.FaceImageInfo);

        // Assert
        faceImageInfo.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Code.ShouldBe("A0001"),
            static sut => sut.FileName.ShouldBe("A0001.jpg"),
            static sut => sut.DownloadUrl.ToString().ShouldBe("https://example.kaonavi.jp/image/xxxx.jpg?Expires=1755255000&Signature=xxxx&Key-Pair-Id=EXAMPLEKEYPAIRID"),
            static sut => sut.UpdatedAt.ShouldBe(new DateTime(2020, 10, 1, 1, 23, 45))
        );
    }
}
