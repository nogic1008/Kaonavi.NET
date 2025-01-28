using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

/// <summary>カオナビ API v2 を呼び出すサービスの実装</summary>
public partial class KaonaviClient : IDisposable, IKaonaviClient
{
    /// <summary>カオナビ API v2 のルートアドレス</summary>
    private const string BaseApiAddress = "https://api.kaonavi.jp/api/v2.0/";

    /// <summary>更新リクエストの制限数</summary>
    /// <seealso href="https://developer.kaonavi.jp/api/v2.0/index.html#section/%E3%83%AA%E3%82%AF%E3%82%A8%E3%82%B9%E3%83%88%E5%88%B6%E9%99%90"/>
    private const int UpdateRequestLimit = 5;

    /// <summary>更新リクエストがリセットされる時間(秒)</summary>
    /// <seealso href="https://developer.kaonavi.jp/api/v2.0/index.html#section/%E3%83%AA%E3%82%AF%E3%82%A8%E3%82%B9%E3%83%88%E5%88%B6%E9%99%90"/>
    private const int WaitSeconds = 60;

    /// <summary><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='client']"/></summary>
    private readonly HttpClient _client;

    /// <summary><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerKey']"/></summary>
    private readonly string _consumerKey;

    /// <summary><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></summary>
    private readonly string _consumerSecret;

    /// <summary><inheritdoc cref="KaonaviClient(HttpClient, string, string, TimeProvider)" path="/param[@name='timeProvider']"/></summary>
    private readonly TimeProvider _timeProvider;

    #region Properties
    private const string TokenHeader = "Kaonavi-Token";
    /// <summary>
    /// アクセストークン文字列を取得または設定します。
    /// 各種API呼び出し時、この項目が<see langword="null"/>の場合は自動的に<see cref="AuthenticateAsync"/>を呼び出します。
    /// </summary>
    public string? AccessToken
    {
        get => _client.DefaultRequestHeaders.TryGetValues(TokenHeader, out var values) ? values.First() : null;
        set
        {
            _ = _client.DefaultRequestHeaders.Remove(TokenHeader);
            if (!string.IsNullOrWhiteSpace(value))
                _client.DefaultRequestHeaders.Add(TokenHeader, value);
        }
    }

    private const string DryRunHeader = "Dry-Run";
    /// <summary>
    /// dryrunモードの動作有無を取得または設定します。
    /// <c>true</c>に設定することで、データベースの操作を行わず、リクエストの内容が適切であるかを検証することが出来ます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#section/dryrun"/>
    /// </summary>
    public bool UseDryRun
    {
        get => _client.DefaultRequestHeaders.TryGetValues(DryRunHeader, out var values) && values.First() == "1";
        set
        {
            _ = _client.DefaultRequestHeaders.Remove(DryRunHeader);
            if (value)
                _client.DefaultRequestHeaders.Add(DryRunHeader, "1");
        }
    }

    /// <summary>更新リクエストの呼び出し履歴</summary>
    private readonly ConcurrentQueue<ITimer> _requestQueues = new();
    /// <summary>更新リクエストの呼び出し制限管理</summary>
    private readonly SemaphoreSlim _semaphore = new(UpdateRequestLimit, UpdateRequestLimit);

    /// <summary>
    /// 更新リクエストの呼び出し回数
    /// </summary>
    /// <seealso href="https://developer.kaonavi.jp/api/v2.0/index.html#section/%E3%83%AA%E3%82%AF%E3%82%A8%E3%82%B9%E3%83%88%E5%88%B6%E9%99%90"/>
    public int UpdateRequestCount => UpdateRequestLimit - _semaphore.CurrentCount;
    #endregion Properties

    /// <summary>
    /// KaonaviClientの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="client">APIコール時に利用する<see cref="HttpClient"/>のインスタンス</param>
    /// <param name="consumerKey">Consumer Key</param>
    /// <param name="consumerSecret">Consumer Secret</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="client"/>, <paramref name="consumerKey"/>または<paramref name="consumerSecret"/>が<see langword="null"/>の場合にスローされます。
    /// </exception>
    public KaonaviClient(HttpClient client, string consumerKey, string consumerSecret) : this(client, consumerKey, consumerSecret, TimeProvider.System) { }

    /// <inheritdoc cref="KaonaviClient(HttpClient, string, string)"/>
    /// <param name="client"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='client']"/></param>
    /// <param name="consumerKey"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerKey']"/></param>
    /// <param name="consumerSecret"><inheritdoc cref="KaonaviClient(HttpClient, string, string)" path="/param[@name='consumerSecret']"/></param>
    /// <param name="timeProvider">時間の抽象化クラス(更新リクエストの呼び出し回数制限に使用)</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="client"/>, <paramref name="consumerKey"/>, <paramref name="consumerSecret"/>または<paramref name="timeProvider"/>が<see langword="null"/>の場合にスローされます。
    /// </exception>
    internal KaonaviClient(HttpClient client, string consumerKey, string consumerSecret, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(consumerKey);
        ArgumentNullException.ThrowIfNull(consumerSecret);
        ArgumentNullException.ThrowIfNull(timeProvider);

        (_client, _consumerKey, _consumerSecret, _timeProvider) = (client, consumerKey, consumerSecret, timeProvider);
        _client.BaseAddress ??= new(BaseApiAddress);
    }

    #region IDisposable
    /// <summary>このインスタンスが破棄済みかどうかを表す値。</summary>
    private bool _disposedValue;

    /// <summary>
    /// <see cref="KaonaviClient"/>で使用しているリソースを解放します。
    /// </summary>
    /// <param name="disposing">マネージド リソースを解放する場合は<see langword="true"/>。</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _semaphore.Dispose();
                while (_requestQueues.TryDequeue(out var timer))
                    timer.Dispose();
                // HttpClientは外部から渡されたものなので、ここでDisposeしない
            }
            _disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// このインスタンスが破棄済みの場合に<see cref="ObjectDisposedException"/>をスローします。
    /// </summary>
    /// <exception cref="ObjectDisposedException">このインスタンスがすでに破棄されている場合にスローされます。</exception>
    private void ThrowIfDisposed()
#if NET8_0_OR_GREATER
        => ObjectDisposedException.ThrowIf(_disposedValue, GetType().FullName!);
#else
    {
        if (_disposedValue)
            throw new ObjectDisposedException(GetType().FullName);
    }
#endif
    #endregion IDisposable

    /// <summary>
    /// アクセストークンを発行します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E3%83%88%E3%83%BC%E3%82%AF%E3%83%B3/paths/~1token/post"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <inheritdoc cref="ThrowIfDisposed" path="/exception"/>
    public async ValueTask<Token> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        byte[] byteArray = Encoding.UTF8.GetBytes($"{_consumerKey}:{_consumerSecret}");
        var content = new FormUrlEncodedContent([new("grant_type", "client_credentials")]);
        _client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(byteArray));

        var response = await _client.PostAsync("token", content, cancellationToken).ConfigureAwait(false);
        await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

        var token = await response.Content
            .ReadFromJsonAsync(Context.Default.Token, cancellationToken)
            .ConfigureAwait(false);
        _client.DefaultRequestHeaders.Authorization = null;
        return token!;
    }

    /// <summary>APIコール前に必要な認証を行います。</summary>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    private async ValueTask FetchTokenAsync(CancellationToken cancellationToken)
        => AccessToken ??= (await AuthenticateAsync(cancellationToken).ConfigureAwait(false)).AccessToken;

    /// <summary>
    /// APIを呼び出します。
    /// </summary>
    /// <param name="request">APIに対するリクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <inheritdoc cref="ThrowIfDisposed" path="/exception"/>
    /// <inheritdoc cref="ValidateApiResponseAsync" path="/exception"/>
    private async ValueTask<HttpResponseMessage> CallApiAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await FetchTokenAsync(cancellationToken).ConfigureAwait(false);
        var response = await _client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);
        return response;
    }

    /// <summary>
    /// APIを呼び出し、受け取ったJSONを<typeparamref name="T"/>に変換して返します。
    /// </summary>
    /// <typeparam name="T">JSONの型</typeparam>
    /// <param name="request">APIに対するリクエスト</param>
    /// <param name="typeInfo">レスポンスをJSONに変換するためのメタ情報</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <inheritdoc cref="CallApiAsync" path="/exception"/>
    private async ValueTask<T> CallApiAsync<T>(HttpRequestMessage request, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken)
    {
        var response = await CallApiAsync(request, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync(typeInfo, cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>
    /// APIを呼び出し、受け取った配列をラップしたJSONオブジェクトを<typeparamref name="T"/>に変換して返します。
    /// </summary>
    /// <typeparam name="T">JSONの型</typeparam>
    /// <param name="request">APIに対するリクエスト</param>
    /// <param name="propertyName">配列が格納されたJSONのプロパティ名</param>
    /// <param name="typeInfo">レスポンスをJSONに変換するためのメタ情報</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <inheritdoc cref="CallApiAsync" path="/exception"/>
    private async ValueTask<T> CallApiAsync<T>(HttpRequestMessage request, string propertyName, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken)
    {
        var response = await CallApiAsync(request, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadFromJsonAsync(Context.Default.JsonElement, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize(json.GetProperty(propertyName), typeInfo)!;
    }

    /// <summary>
    /// 受け取った配列をラップしたJSONオブジェクトをBodyとしてAPIを呼び出し、受け取った<inheritdoc cref="TaskProgress" path="/param[@name='Id']"/>を返します。
    /// </summary>
    /// <typeparam name="T">リクエストBodyの型</typeparam>
    /// <param name="method">HTTP Method</param>
    /// <param name="uri">リクエストURI</param>
    /// <param name="payload">APIに対するリクエスト</param>
    /// <param name="utf8PropertyName">配列が格納されたJSONのプロパティ名</param>
    /// <param name="typeInfo"><paramref name="payload"/>をJSONに変換するためのメタ情報</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
    /// <inheritdoc cref="CallApiAsync" path="/exception"/>
    /// <inheritdoc cref="ThrowIfDisposed" path="/exception"/>
    private ValueTask<int> CallTaskApiAsync<T>(HttpMethod method, string uri, T payload, ReadOnlySpan<byte> utf8PropertyName, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        writer.WriteStartObject();
        writer.WritePropertyName(utf8PropertyName);
        JsonSerializer.Serialize(writer, payload, typeInfo);
        writer.WriteEndObject();
        writer.Flush();

        return CallRequestLimitApiAsync(new(method, uri)
        {
            Content = new ReadOnlyMemoryContent(buffer.WrittenMemory)
            {
                Headers = { ContentType = new("application/json") }
            }
        }, cancellationToken);
    }

    /// <summary>
    /// 更新リクエスト制限のあるAPIを呼び出します。
    /// 制限を超える場合は、呼び出し可能になるまで待機します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#section/%E3%83%AA%E3%82%AF%E3%82%A8%E3%82%B9%E3%83%88%E5%88%B6%E9%99%90"/>
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async ValueTask<int> CallRequestLimitApiAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        ITimer? timer = null;
        try
        {
            int taskId = (await CallApiAsync(request, Context.Default.JsonElement, cancellationToken).ConfigureAwait(false)).GetProperty("task_id"u8).GetInt32();

            timer = _timeProvider.CreateTimer(OnFinished, null, TimeSpan.FromSeconds(WaitSeconds), Timeout.InfiniteTimeSpan);
            _requestQueues.Enqueue(timer);
            return taskId;
        }
        catch
        {
            timer?.Dispose();
            _semaphore.Release();
            throw;
        }

        void OnFinished(object? _)
        {
            if (_requestQueues.TryDequeue(out var timer))
                timer.Dispose();
            _semaphore.Release();
        }
    }

    /// <summary>
    /// APIが正しく終了したかどうかを検証します。
    /// エラーが返ってきた場合は、エラーメッセージを取得し例外をスローします。
    /// </summary>
    /// <param name="response">APIレスポンス</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <exception cref="ApplicationException">
    /// APIからのHTTPステータスコードが200-299番でない場合にスローされます。
    /// </exception>
    private static async ValueTask ValidateApiResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            string errorMessage = response.Content.Headers.ContentType!.MediaType == "application/json"
                // { "errors": ["エラーメッセージ1", "エラーメッセージ2",...] }
                ? string.Join("\n", (await response.Content.ReadFromJsonAsync(Context.Default.JsonElement, cancellationToken)).GetProperty("errors"u8).EnumerateArray().Select(e => e.GetString()))
                : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new ApplicationException(errorMessage, ex);
        }
    }
}
