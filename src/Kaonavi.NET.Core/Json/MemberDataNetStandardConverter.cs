#if NETSTANDARD2_1
using System.Globalization;
using System.Text.Json.Serialization.Metadata;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Json;

/// <summary>.NET Standard 2.1向けの<see cref="MemberData"/>用JSON変換</summary>
internal sealed class MemberDataNetStandardConverter : JsonConverter<MemberData>
{
    /// <inheritdoc/>
    public override MemberData Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var typeInfo =
            (JsonTypeInfo<MemberDataNetStandardJson>)
                options.GetTypeInfo(typeof(MemberDataNetStandardJson));
        var data = JsonSerializer.Deserialize(ref reader, typeInfo) ?? throw new JsonException();

        return new(
            data.Code,
            data.Name,
            data.NameKana,
            data.Mail,
            ParseDate(data.EnteredDate),
            ParseDate(data.RetiredDate),
            data.Gender,
            ParseDate(data.Birthday),
            data.Department,
            data.SubDepartments,
            data.CustomFields
        );
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        MemberData value,
        JsonSerializerOptions options
    )
    {
        var data = new MemberDataNetStandardJson(
            value.Code,
            value.Name,
            value.NameKana,
            value.Mail,
            FormatDate(value.EnteredDate),
            FormatDate(value.RetiredDate),
            value.Gender,
            FormatDate(value.Birthday),
            value.Department,
            value.SubDepartments,
            value.CustomFields
        );
        var typeInfo =
            (JsonTypeInfo<MemberDataNetStandardJson>)
                options.GetTypeInfo(typeof(MemberDataNetStandardJson));
        JsonSerializer.Serialize(writer, data, typeInfo);
    }

    private static DateOnly? ParseDate(string? value) =>
        string.IsNullOrEmpty(value) ? null : DateOnly.Parse(value);

    private static string? FormatDate(DateOnly? value) =>
        value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}

/// <summary><see cref="MemberData"/>の.NET Standard 2.1向けJSONマッピングモデル</summary>
internal sealed record MemberDataNetStandardJson(
    string Code,
    string? Name = null,
    string? NameKana = null,
    string? Mail = null,
    string? EnteredDate = null,
    string? RetiredDate = null,
    string? Gender = null,
    string? Birthday = null,
    MemberDepartment? Department = null,
    IReadOnlyList<MemberDepartment>? SubDepartments = null,
    IReadOnlyList<CustomFieldValue>? CustomFields = null
);
#endif
