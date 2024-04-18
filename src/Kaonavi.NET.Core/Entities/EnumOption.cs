namespace Kaonavi.Net.Entities;

/// <summary>マスター情報</summary>
/// <param name="SheetName">シート名</param>
/// <param name="Id">
/// カスタムフィールドID <seealso cref="CustomFieldLayout.Id"/>
/// </param>
/// <param name="Name">カスタムフィールド名</param>
/// <param name="EnumOptionData"><see cref="FieldLayout.Enum"/></param>
public record EnumOption(
    string SheetName,
    int Id,
    string Name,
    IReadOnlyList<EnumOption.Data> EnumOptionData
)
{
    /// <summary>マスター項目値</summary>
    /// <param name="Id">マスターID</param>
    /// <param name="Name">マスター名</param>
    public record Data(int Id, string Name);
}
