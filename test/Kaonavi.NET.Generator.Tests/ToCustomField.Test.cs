using System.Globalization;
using Kaonavi.Net.Generator.Tests.Entities;
using RandomFixtureKit;

namespace Kaonavi.Net.Generator.Tests;

/// <summary><see cref="SheetDataGenerator"/>で生成される<see cref="ISheetData.ToCustomFields"/>メソッドのテスト</summary>
[TestClass, TestCategory("Source Generator")]
public sealed class ToCustomFieldTest
{
    /// <summary>
    /// <see cref="NormalClassSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [TestMethod(DisplayName = $"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つクラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public void Class_Generates_ToCustomFields_Method()
    {
        // Arrange
        var values = FixtureFactory.CreateMany<NormalClassSheetData>(10);

        // Act - Assert
        foreach (var sut in values)
        {
            sut.ToCustomFields().ShouldBe([
                new(101, sut.Name!),
                new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
            ]);
        }
    }

    /// <summary>
    /// <see cref="NormalRecordSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [TestMethod(DisplayName = $"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ record クラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public void Record_Generates_ToCustomFields_Method()
    {
        // Arrange
        var values = FixtureFactory.CreateMany<NormalRecordSheetData>(10);

        // Act - Assert
        foreach (var sut in values)
        {
            sut.ToCustomFields().ShouldBe([
                new(101, sut.Name!),
                new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            ]);
        }
    }

    /// <summary>
    /// <see cref="NoNamespaceClassSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [TestMethod(DisplayName = $"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ名前空間を持たないクラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public void Class_Without_Namespace_Generates_ToCustomFields_Method()
    {
        // Arrange
        var values = FixtureFactory.CreateMany<NoNamespaceClassSheetData>(10);

        // Act - Assert
        foreach (var sut in values)
        {
            sut.ToCustomFields().ShouldBe([
                new(101, sut.Name),
                new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            ]);
        }
    }

    /// <summary>
    /// <see cref="NoNamespaceRecordSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [TestMethod(DisplayName = $"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ名前空間を持たない record クラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public void Record_Without_Namespace_Generates_ToCustomFields_Method()
    {
        // Arrange
        var values = FixtureFactory.CreateMany<NoNamespaceRecordSheetData>(10);

        // Act - Assert
        foreach (var sut in values)
        {
            sut.ToCustomFields().ShouldBe([
                new(101, sut.Name),
                new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            ]);
        }
    }
}
