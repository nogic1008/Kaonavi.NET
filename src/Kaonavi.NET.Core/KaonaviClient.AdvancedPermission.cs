using System.Runtime.CompilerServices;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.IAdvancedPermission
{
    /// <summary>
    /// 拡張アクセス設定 API
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%8B%A1%E5%BC%B5%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E8%A8%AD%E5%AE%9A"/>
    /// </summary>
    public interface IAdvancedPermission
    {
        /// <summary>
        /// <inheritdoc cref="AdvancedPermission" path="/summary"/>の一覧を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%8B%A1%E5%BC%B5%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E8%A8%AD%E5%AE%9A/paths/~1advanced_permissions~1{advanced_type}/get"/>
        /// </summary>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<IReadOnlyList<AdvancedPermission>> ListAsync(AdvancedType type, CancellationToken cancellationToken = default);

        /// <summary>
        /// 現在登録されている<inheritdoc cref="AdvancedPermission" path="/summary"/>を全て、リクエストしたデータで入れ替えます。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%8B%A1%E5%BC%B5%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E8%A8%AD%E5%AE%9A/paths/~1advanced_permissions~1{advanced_type}/put"/>
        /// </summary>
        /// <remarks>
        /// 1ユーザーあたりの上限
        /// <list type="bullet">
        /// <item>
        ///   <term><inheritdoc cref="AdvancedType.Member" path="/summary"/></term>
        ///   <description>追加・除外 各30,000件</description>
        /// </item>
        /// <item>
        ///   <term><inheritdoc cref="AdvancedType.Department" path="/summary"/></term>
        ///   <description>追加・除外 各1,000件</description>
        /// </item>
        /// </list>
        /// 更新リクエスト制限の対象APIです。
        /// </remarks>
        /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
        /// <param name="payload">入れ替え対象となるデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<int> ReplaceAsync(AdvancedType type, IReadOnlyList<AdvancedPermission> payload, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public IAdvancedPermission AdvancedPermission => this;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の<see cref="AdvancedType"/>である場合にスローされます。</exception>
    ValueTask<IReadOnlyList<AdvancedPermission>> IAdvancedPermission.ListAsync(AdvancedType type, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, $"advanced_permissions/{AdvancedTypeToString(type)}"), "advanced_permission_data", Context.Default.IReadOnlyListAdvancedPermission, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の<see cref="AdvancedType"/>である場合にスローされます。</exception>
    ValueTask<int> IAdvancedPermission.ReplaceAsync(AdvancedType type, IReadOnlyList<AdvancedPermission> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Put, $"advanced_permissions/{AdvancedTypeToString(type)}", payload, "advanced_permission_data"u8, Context.Default.IReadOnlyListAdvancedPermission, cancellationToken);

    /// <summary>
    /// <see cref="AdvancedType"/> -&gt; <see langword="string"/>変換
    /// </summary>
    /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
    /// <param name="argument">引数</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の<see cref="AdvancedType"/>である場合にスローされます。</exception>
    private static string AdvancedTypeToString(AdvancedType type, [CallerArgumentExpression(nameof(type))] string? argument = null) => type switch
    {
        AdvancedType.Member => "member",
        AdvancedType.Department => "department",
        _ => throw new ArgumentOutOfRangeException(argument),
    };
}
