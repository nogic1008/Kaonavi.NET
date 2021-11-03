using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>レイアウト定義 カスタム列項目</summary>
    /// <param name="Id">シート項目ID</param>
    /// <param name="Name"><inheritdoc cref="Field" path="/param[@name='Name']/text()"/></param>
    /// <param name="Required"><inheritdoc cref="Field" path="/param[@name='Required']/text()"/></param>
    /// <param name="Type"><inheritdoc cref="Field" path="/param[@name='Type']/text()"/></param>
    /// <param name="MaxLength"><inheritdoc cref="Field" path="/param[@name='MaxLength']/text()"/></param>
    /// <param name="Enum"><inheritdoc cref="Field" path="/param[@name='Enum']/text()"/></param>
    public record CustomField(
        [property: JsonPropertyName("id")] int Id,
        string Name,
        bool Required,
        FieldType Type,
        int? MaxLength,
        IReadOnlyList<string?> Enum
    ) : Field(Name, Required, Type, MaxLength, Enum);
}
