using System.Globalization;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Generator.Tests.Entities;
using RandomFixtureKit;

namespace Kaonavi.Net.Generator.Tests;

/// <summary><see cref="SheetDataGenerator"/>で生成される<see cref="ISheetData.ToCustomFields"/>メソッドのテスト</summary>
[Category("Source Generator")]
public sealed class ToCustomFieldsTest
{
    /// <summary>
    /// <see cref="ISheetData.ToCustomFields"/>の戻り値検証用メソッド
    /// </summary>
    /// <typeparam name="T"><see cref="ISheetData.ToCustomFields"/>を持つクラス</typeparam>
    /// <param name="value">検証するインスタンス</param>
    /// <param name="expected">期待される<see cref="ISheetData.ToCustomFields"/>の戻り値</param>
    /// <returns></returns>
    private async static Task AssertCustomFields<T>(T value, IReadOnlyList<CustomFieldValue> expected) where T : ISheetData
        => await Assert.That(value.ToCustomFields()).IsEquivalentTo(expected);

    /// <summary>
    /// <see cref="NormalClassSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [Test($"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つクラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public async Task Class_Generates_ToCustomFields_Method()
        => await Task.WhenAll(
            FixtureFactory.CreateMany<NormalClassSheetData>(10)
                .Select(async sut => AssertCustomFields(sut, [
                    new(101, sut.Name!),
                    new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                ])
            )
        );

    /// <summary>
    /// <see cref="NormalRecordSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [Test($"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ record クラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public async Task Record_Generates_ToCustomFields_Method()
        => await Task.WhenAll(
            FixtureFactory.CreateMany<NormalRecordSheetData>(10)
                .Select(async sut => AssertCustomFields(sut, [
                    new(101, sut.Name!),
                    new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                ])
            )
        );

    /// <summary>
    /// <see cref="NoNamespaceClassSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [Test($"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ名前空間を持たないクラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public async Task Class_Without_Namespace_Generates_ToCustomFields_Method()
        => await Task.WhenAll(
            FixtureFactory.CreateMany<NoNamespaceClassSheetData>(10)
                .Select(async sut => AssertCustomFields(sut, [
                    new(101, sut.Name!),
                    new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                ])
            )
        );

    /// <summary>
    /// <see cref="NoNamespaceRecordSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [Test($"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ名前空間を持たない record クラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public async Task Record_Without_Namespace_Generates_ToCustomFields_Method()
        => await Task.WhenAll(
            FixtureFactory.CreateMany<NoNamespaceRecordSheetData>(10)
                .Select(async sut => AssertCustomFields(sut, [
                    new(101, sut.Name!),
                    new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                ])
            )
        );
}
