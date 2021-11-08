namespace ConsoleAppSample;

/// <summary>
/// <see cref="Kaonavi.Net.Services.KaonaviV2Service"/>に渡す設定項目。
/// </summary>
public class KaonaviOptions
{
#nullable disable warnings
    /// <summary>Consumer Key</summary>
    public string ConsumerKey { get; set; }
    /// <summary>Consumer Secret</summary>
    public string ConsumerSecret { get; set; }
#nullable restore
    /// <summary>
    /// <see cref="Kaonavi.Net.Services.KaonaviV2Service.UseDryRun"/>
    /// </summary>
    public bool UseDryRun { get; set; }
}
