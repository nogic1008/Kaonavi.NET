using System.Diagnostics.CodeAnalysis;

namespace Kaonavi.Net;

/// <summary>
/// Kaonavi.NET.Generatorに、このエンティティの<see cref="ISheetData.ToCustomFields"/>を自動でソース生成することを指示します。
/// </summary>
[ExcludeFromCodeCoverage] // Only use on source generator
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SheetSerializableAttribute : Attribute;

