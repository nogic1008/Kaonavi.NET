namespace Kaonavi.Net;

/// <summary>
/// Kaonavi.NET.Generatorに、このエンティティの<see cref="ISheetData.ToCustomFields"/>を自動でソース生成することを指示します。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SheetSerializableAttribute : Attribute;

/// <summary>
/// このプロパティが<see cref="Entities.CustomFieldValue"/>の一部であることを示します。
/// </summary>
/// <param name="id"><inheritdoc cref="Entities.CustomFieldValue.Id" path="/summary"/></param>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class CustomFieldAttribute(int id) : Attribute
{
    /// <inheritdoc cref="Entities.CustomFieldValue.Id"/>
    public int Id { get; } = id;
}
