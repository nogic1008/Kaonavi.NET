using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="AttachmentPayload"/>の単体テスト</summary>
[Category("Entities")]
public sealed class AttachmentPayloadTest
{
    /// <summary>JSONからデシリアライズできる。</summary>
    [Test($"{nameof(AttachmentPayload)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CanDeserializeJSON()
    {
        // Arrange
        // lang=json,strict
        var json = """
        {
          "code": "A0001",
          "records": [
            {
              "file_name": "sample.txt",
              "base64_content": "44GT44KM44Gv44K144Oz44OX44Or44OG44Kt44K544OI44Gn44GZ44CC"
            }
          ]
        }
        """u8;

        // Act
        var attachment = JsonSerializer.Deserialize(json, JsonContext.Default.AttachmentPayload);

        // Assert
        await Assert.That(attachment).IsNotNull()
            .And.Member(static o => o.Code, static o => o.IsEqualTo<string>("A0001"))
            .And.Member(static o => o.Records.Count, static o => o.IsEqualTo(1))
            .And.Member(static o => o.Records[0].FileName, static o => o.IsEqualTo<string>("sample.txt"))
            .And.Member(static o => o.Records[0].Content, static o => o.IsEquivalentTo(Convert.FromBase64String("44GT44KM44Gv44K144Oz44OX44Or44OG44Kt44K544OI44Gn44GZ44CC")));
    }
}
