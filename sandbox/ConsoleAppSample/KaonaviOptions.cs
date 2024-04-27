namespace ConsoleAppSample;

/// <summary>
/// <see cref="Kaonavi.Net.KaonaviClient"/>に渡す設定項目。
/// </summary>
public class KaonaviOptions
{
    /// <summary>
    /// <inheritdoc cref="Kaonavi.Net.KaonaviClient.KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerSecret']"/>
    /// </summary>
    public required string ConsumerKey { get; set; }

    /// <summary>
    /// <inheritdoc cref="Kaonavi.Net.KaonaviClient.KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerSecret']"/>
    /// </summary>
    public required string ConsumerSecret { get; set; }

    /// <summary>
    /// <see cref="Kaonavi.Net.KaonaviClient.UseDryRun"/>
    /// </summary>
    public bool UseDryRun { get; set; }
}
