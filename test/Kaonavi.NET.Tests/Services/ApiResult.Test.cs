namespace Kaonavi.Net.Tests.Services;

using Kaonavi.Net.Services;

/// <summary>
/// <see cref="ApiResult{T}"/>の単体テスト
/// </summary>
public class ApiResultTest
{
    internal record SampleClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    /// <summary>
    /// 正しくないJSONの場合に、<see cref="JsonException"/>の例外をスローする。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    [Theory]
    [InlineData("[]")]
    [InlineData("{\"foo\":[]}")]
    [InlineData("{\"sample_data\":{}}")]
    [InlineData("{\"sample_data\":[{\"id\":\"bar\"}]}")]
    public void CannotDeserializeJson(string json)
    {
        Action deserialize = () => JsonSerializer.Deserialize<ApiResult<SampleClass>>(json, JsonConfig.Default);
        deserialize.Should().ThrowExactly<JsonException>();
    }

    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    /// <param name="json">JSON文字列</param>
    /// <param name="propertyName"><see cref="ApiResult{T}.PropertyName"/></param>
    /// <param name="id"><see cref="SampleClass.Id"/></param>
    /// <param name="name"><see cref="SampleClass.Name"/></param>
    [Theory]
    [InlineData("{\"sample_data\":[{\"id\":1,\"name\":\"foo\"}]}", "sample_data", 1, "foo")]
    [InlineData("{\"other_data\":[{\"id\":2,\"name\":\"bar\"}],\"ignore\":true}", "other_data", 2, "bar")]
    public void CanDeserializeJson(string json, string propertyName, int id, string name)
    {
        var result = JsonSerializer.Deserialize<ApiResult<SampleClass>>(json, JsonConfig.Default)!;
        result.PropertyName.Should().Be(propertyName);
        result.Data.Should().Equal(new SampleClass { Id = id, Name = name });
    }
}
