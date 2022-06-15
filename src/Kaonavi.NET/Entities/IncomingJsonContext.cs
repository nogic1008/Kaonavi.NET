namespace Kaonavi.Net.Entities;

[JsonSerializable(typeof(Role))]
[JsonSerializable(typeof(Token))]
[JsonSerializable(typeof(User))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
internal partial class IncomingJsonContext : JsonSerializerContext
{
}
