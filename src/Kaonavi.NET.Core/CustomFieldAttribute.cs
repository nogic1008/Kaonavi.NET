using System.Diagnostics.CodeAnalysis;

namespace Kaonavi.Net;

/// <summary>
/// このプロパティが<see cref="Entities.CustomFieldValue"/>の一部であることを示します。
/// </summary>
/// <param name="id"><inheritdoc cref="Entities.CustomFieldValue.Id" path="/summary"/></param>
[ExcludeFromCodeCoverage] // Only use on source generator
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class CustomFieldAttribute(int id) : Attribute
{
    /// <inheritdoc cref="Entities.CustomFieldValue.Id"/>
    public int Id { get; } = id;
}
