namespace Kaonavi.Net.Entities;

/// <summary>レイアウト定義 列項目</summary>
/// <param name="Name">項目名</param>
/// <param name="Required">必須入力項目</param>
/// <param name="Type">入力タイプ</param>
/// <param name="MaxLength">
/// <paramref name="Type"/>が<see cref="FieldType.String"/>の場合に設定可能な最大文字数
/// <para><see cref="FieldType.String"/>以外の場合は<see langword="null"/>を設定して返却します。</para>
/// </param>
/// <param name="Enum">
/// <paramref name="Type"/>が<see cref="FieldType.Enum"/>の場合に設定可能な値のリスト
/// </param>
/// <param name="ReadOnly">
/// 読み取り専用
/// <para>
/// APIで更新不可の項目に付与されます。
/// 更新不可の項目は <see href="https://developer.kaonavi.jp/api/v2.0/index.html#section/custom_fields">custom_fields について</see>を参照ください。
/// </para>
/// </param>
public record FieldLayout(
    string Name,
    bool Required,
    FieldType Type,
    int? MaxLength,
    IReadOnlyList<string?> Enum,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] bool ReadOnly = false
);
