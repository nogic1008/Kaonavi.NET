using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FaceImage"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class FaceImageTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [TestMethod($"{nameof(FaceImage)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeJSON()
    {
        // Arrange
        // lang=json,strict
        const string json = """
        {
          "code": "A0001",
          "base64_face_image": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAA1JREFUGFdj+P///38ACfsD/QVDRcoAAAAASUVORK5CYII="
        }
        """;

        // Act
        var faceImage = JsonSerializer.Deserialize(json, Context.Default.FaceImage);

        // Assert
        faceImage.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Code.ShouldBe("A0001"),
            static sut => sut.Content.ShouldBe(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAA1JREFUGFdj+P///38ACfsD/QVDRcoAAAAASUVORK5CYII="))
        );
    }
}
