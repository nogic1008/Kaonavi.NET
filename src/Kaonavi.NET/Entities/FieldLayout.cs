namespace Kaonavi.Net.Entities;

/// <summary>レイアウト定義 列項目</summary>
/// <param name="Name">項目名</param>
/// <param name="Required">必須入力項目</param>
/// <param name="Type">入力タイプ</param>
/// <param name="MaxLength">
/// <paramref name="Type"/>が<see cref="FieldType.String"/>の場合に設定可能な最大文字数
/// </param>
/// <param name="Enum">
/// <paramref name="Type"/>が<see cref="FieldType.Enum"/>の場合に設定可能な値のリスト
/// </param>
public record FieldLayout(
    string Name,
    bool Required,
    FieldType Type,
    int? MaxLength,
    IReadOnlyCollection<string?> Enum
);
