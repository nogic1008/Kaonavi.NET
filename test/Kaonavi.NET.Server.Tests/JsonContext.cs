using System.Text.Json.Serialization;

namespace Kaonavi.Net.Server.Tests;

[JsonSerializable(typeof(KaonaviWebhook))]
public partial class Context : JsonSerializerContext;
