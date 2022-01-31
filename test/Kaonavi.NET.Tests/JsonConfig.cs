using Kaonavi.Net.Services;

namespace Kaonavi.Net.Tests;

internal static class JsonConfig
{
    internal static readonly JsonSerializerOptions Default = new(KaonaviV2Service.Options);
}
