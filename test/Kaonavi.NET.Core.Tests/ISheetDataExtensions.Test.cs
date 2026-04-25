using Kaonavi.Net.Entities;
using RandomFixtureKit;

namespace Kaonavi.Net.Tests;

/// <summary><see cref="ISheetDataExtensions"/>の単体テスト</summary>
[Category("Entities")]
public sealed class ISheetDataExtensionsTest
{
    public class TestSheetData : ISheetData
    {
        public required string Code { get; init; }
        public required string Name { get; init; }
        public IReadOnlyList<CustomFieldValue> ToCustomFields() => [new(101, Name)];
    }

    /// <summary>
    /// <see cref="ISheetDataExtensions.ToSingleSheetData{T}(T)"/>は、単一レコードであるSheetDataの一覧を返す。
    /// </summary>
    [Test($"{nameof(ISheetDataExtensions)}.{nameof(ISheetDataExtensions.ToSingleSheetData)}() > 単一レコードであるSheetDataの一覧を返す。")]
    public async Task ToSingleSheetData_Returns_Single_SheetData()
    {
        // Arrange
        const int length = 10;
        var values = FixtureFactory.CreateMany<TestSheetData>(length);

        // Act
        var actual = values.ToSingleSheetData();

        // Assert
        await Assert.That(actual).Count().IsEqualTo(length)
            .And.All(static o => o.Records.Count == 1);
    }

    /// <summary>
    /// <see cref="ISheetDataExtensions.ToMultipleSheetData{T}(IEnumerable{T})"/>は、複数レコードであるSheetDataの一覧を返す。
    /// </summary>
    [Test($"{nameof(ISheetDataExtensions)}.{nameof(ISheetDataExtensions.ToMultipleSheetData)}() > 複数レコードであるSheetDataの一覧を返す。")]
    public async Task ToMultipleSheetData_Returns_Multiple_SheetData()
    {
        // Arrange
        const int codeLength = 10;
        string[] codes = FixtureFactory.CreateMany<string>(codeLength);
        const int nameLength = 3;
        string[] names = FixtureFactory.CreateMany<string>(nameLength);
        var values = codes.SelectMany(code => names.Select(name => new TestSheetData { Code = code, Name = name }));

        // Act
        var actual = values.ToMultipleSheetData();

        // Assert
        await Assert.That(actual).IsNotNull()
            .And.Count().IsEqualTo(codeLength)
            .And.All(static o => o.Records.Count == nameLength);
    }
}
