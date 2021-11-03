using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>マスター情報</summary>
    /// <param name="SheetName">シート名</param>
    /// <param name="Id">
    /// カスタムフィールドID <seealso cref="CustomField.Id"/>
    /// </param>
    /// <param name="Name">カスタムフィールド名</param>
    /// <param name="EnumOptionData"><see cref="Field.Enum"/></param>
    public record EnumOption(
        [property: JsonPropertyName("sheet_name")] string SheetName,
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("enum_option_data")] IReadOnlyList<EnumOption.Data> EnumOptionData
    )
    {
        /// <summary>マスター項目値</summary>
        /// <param name="Id">マスターID</param>
        /// <param name="Name">マスター名</param>
        public record Data(
            [property: JsonPropertyName("id")] int Id,
            [property: JsonPropertyName("name")] string Name
        );
    }
}
