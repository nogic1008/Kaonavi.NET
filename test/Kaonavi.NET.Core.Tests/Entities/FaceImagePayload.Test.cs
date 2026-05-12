using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="FaceImagePayload"/>の単体テスト</summary>
[Category("Entities")]
public sealed class FaceImagePayloadTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [Test, Category("JSON Deserialize")]
    [DisplayName($"{nameof(FaceImagePayload)} > JSONからデシリアライズできる。")]
    public async Task CanDeserializeJSON()
    {
        // Arrange
        // lang=json,strict
        var json = """
        {
          "code": "A0001",
          "base64_face_image": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAA1JREFUGFdj+P///38ACfsD/QVDRcoAAAAASUVORK5CYII="
        }
        """u8;

        // Act
        var faceImage = JsonSerializer.Deserialize(json, JsonContext.Default.FaceImagePayload);

        // Assert
        const string expectedBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAA1JREFUGFdj+P///38ACfsD/QVDRcoAAAAASUVORK5CYII=";
        await Assert.That(faceImage).IsNotNull()
            .And.Member(static o => o.Code, static o => o.IsEqualTo<string>("A0001"))
            .And.Member(static o => o.Content, static o => o.IsSequenceEqualTo(Convert.FromBase64String(expectedBase64)));
    }
}
