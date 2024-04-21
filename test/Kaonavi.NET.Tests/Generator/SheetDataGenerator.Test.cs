using System.Globalization;
using Kaonavi.Net.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RandomFixtureKit;

namespace Kaonavi.Net.Tests.Generator;

/// <summary><see cref="Generator.SheetDataGenerator"/>の単体テスト</summary>
[TestClass, TestCategory("Source Generator")]
public sealed class SheetDataGeneratorTest
{
    /// <summary>
    /// <paramref name="code"/>をコンパイルした場合、<paramref name="id"/>のコンパイル警告が発生する。
    /// </summary>
    /// <param name="code">ソースコード</param>
    /// <param name="id">コンパイル警告のID</param>
    [TestMethod("Generator > コンパイル警告が発生する。")]
    [DataRow("""
    using Kaonavi.Net;
    
    [SheetSerializable]
    public class Foo : ISheetData
    {
        public string Code { get; set; }
        [CustomField(101)]
        public string Name { get; set; }
    }
    """, "KAONAVI001", DisplayName = "Generator > partial クラスでない場合、KAONAVI001のコンパイル警告が発生する。")]
    [DataRow("""
    using Kaonavi.Net;
    
    [SheetSerializable]
    public partial record Foo(string Code, [property: CustomField(101)] string Name);
    """, "KAONAVI002", DisplayName = $"Generator > {nameof(ISheetData)}を継承していない場合、KAONAVI002のコンパイル警告が発生する。")]
    [DataRow("""
    using Kaonavi.Net;
    using Kaonavi.Net.Entities;
        
    [SheetSerializable]
    public partial record Foo(string Code, [property: CustomField(101)] string Name) : ISheetData
    {
        public IReadOnlyList<CustomFieldValue> ToCustomFields() => [];
    }
    """, "KAONAVI003", DisplayName = $"Generator > {nameof(ISheetData.ToCustomFields)}メソッドを手動で実装している場合、KAONAVI003のコンパイル警告が発生する。")]
    [DataRow("""
    using Kaonavi.Net;
    
    [SheetSerializable]
    public partial record Foo(string Code, string Name) : ISheetData;
    """, "KAONAVI004", DisplayName = $"Generator > {nameof(CustomFieldAttribute)}を設定したプロパティがない場合、KAONAVI004のコンパイル警告が発生する。")]
    [DataRow("""
    using Kaonavi.Net;
    
    [SheetSerializable]
    public partial record Foo(string Code, [property: CustomField(101)] string Name1, [property: CustomField(101)] string Name2) : ISheetData;
    """, "KAONAVI005", DisplayName = $"Generator > {nameof(CustomFieldAttribute)}のidが重複したプロパティがある場合、KAONAVI005のコンパイル警告が発生する。")]
    public void When_Invalid_Code_Compiler_Warns_With_Diagnostic(string code, string id)
    {
        // Arrange - Act
        var warnings = CSharpGeneratorRunner.RunGenerator(code);

        // Assert
        warnings.Select(x => x.Id).Should().Contain(id);
    }

    /// <summary>
    /// <paramref name="code"/>をコンパイルした場合、<paramref name="version"/>に応じたソースが生成される。
    /// </summary>
    /// <param name="code">ソースコード</param>
    /// <param name="version">C# のバージョン</param>
    [TestMethod("Generator > C# バージョンに応じたソースが生成される。")]
    [DataRow("""
    #nullable disable
    using Kaonavi.Net;
    
    namespace Sample
    {
        [SheetSerializable]
        public partial class Foo : ISheetData
        {
            public string Code { get; set; }
            [CustomField(101)]
            public string Name { get; set; }
        }
    }
    """, LanguageVersion.CSharp9, DisplayName = "Generator > C# 9.0 の場合、ネストされた名前空間 および 配列の初期化子を使用してソース生成する。")]
    [DataRow("""
    using Kaonavi.Net;

    namespace Sample;
        
    [SheetSerializable]
    public partial record Foo(string Code, [property: CustomField(101)] string Name) : ISheetData;
    """, LanguageVersion.CSharp11, DisplayName = "Generator > C# 11.0 の場合、ファイルスコープ名前空間 および 配列の初期化子を使用してソース生成する。")]
    [DataRow("""
    using Kaonavi.Net;

    namespace Sample;
        
    [SheetSerializable]
    public partial record Foo(string Code, [property: CustomField(101)] string Name) : ISheetData;
    """, LanguageVersion.CSharp12, DisplayName = "Generator > C# 12.0 の場合、ファイルスコープ名前空間 および コレクション式を使用してソース生成する。")]
    public void Generates_ToCustomFields_Method(string code, LanguageVersion version)
    {
        // Arrange - Act
        var warnings = CSharpGeneratorRunner.RunGenerator(code, version);

        // Assert
        warnings.Should().BeEmpty();
    }

    /// <summary>
    /// <see cref="NormalClassSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [TestMethod($"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つクラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public void Class_Generates_ToCustomFields_Method()
    {
        // Arrange
        var values = FixtureFactory.CreateMany<NormalClassSheetData>(10);

        // Act - Assert
        typeof(NormalClassSheetData).Should().Implement<ISheetData>()
            .And.HaveMethod(nameof(ISheetData.ToCustomFields), []);
        foreach (var sut in values)
        {
            sut.ToCustomFields().Should().Equal(
                new(101, sut.Name!),
                new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            );
        }
    }

    /// <summary>
    /// <see cref="NormalRecordSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [TestMethod($"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ record クラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public void Record_Generates_ToCustomFields_Method()
    {
        // Arrange
        var values = FixtureFactory.CreateMany<NormalRecordSheetData>(10);

        // Act - Assert
        typeof(NormalRecordSheetData).Should().Implement<ISheetData>()
            .And.HaveMethod(nameof(ISheetData.ToCustomFields), []);
        foreach (var sut in values)
        {
            sut.ToCustomFields().Should().Equal(
                new(101, sut.Name!),
                new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            );
        }
    }

    /// <summary>
    /// <see cref="NoNamespaceClassSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [TestMethod($"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ名前空間を持たないクラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public void Class_Without_Namespace_Generates_ToCustomFields_Method()
    {
        // Arrange
        var values = FixtureFactory.CreateMany<NoNamespaceClassSheetData>(10);

        // Act - Assert
        typeof(NoNamespaceClassSheetData).Should().Implement<ISheetData>()
            .And.HaveMethod(nameof(ISheetData.ToCustomFields), []);
        foreach (var sut in values)
        {
            sut.ToCustomFields().Should().Equal(
                new(101, sut.Name),
                new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            );
        }
    }

    /// <summary>
    /// <see cref="NoNamespaceRecordSheetData"/>の<see cref="ISheetData.ToCustomFields"/>メソッドがソース生成される。
    /// </summary>
    [TestMethod($"Generator > [{nameof(SheetSerializableAttribute)}]属性を持つ名前空間を持たない record クラスがある場合、{nameof(ISheetData.ToCustomFields)}()をソース生成する。")]
    public void Record_Without_Namespace_Generates_ToCustomFields_Method()
    {
        // Arrange
        var values = FixtureFactory.CreateMany<NoNamespaceRecordSheetData>(10);

        // Act - Assert
        typeof(NoNamespaceRecordSheetData).Should().Implement<ISheetData>()
            .And.HaveMethod(nameof(ISheetData.ToCustomFields), []);
        foreach (var sut in values)
        {
            sut.ToCustomFields().Should().Equal(
                new(101, sut.Name),
                new(102, sut.Date1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(103, sut.Date2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(104, sut.Date3.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(105, sut.Date4.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(106, sut.Date5.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new(107, sut.Date6.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            );
        }
    }
}
