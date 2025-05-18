using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Kaonavi.Net.Generator.Tests;

/// <summary><see cref="SheetDataGenerator"/>の単体テスト</summary>
[TestClass, TestCategory("Source Generator")]
public sealed class SheetDataGeneratorTest
{
    /// <summary>
    /// <see cref="When_Invalid_Code_Compiler_Warns_With_Diagnostic"/>用のテストデータ
    /// </summary>
    public static IEnumerable<TestDataRow<(string code, string id)>> InvalidCodeTestData
    {
        get
        {
            // lang=c#-test, strict
            const string NotPartialCode = """
            using Kaonavi.Net;
            
            [SheetSerializable]
            public class Foo : ISheetData
            {
                public string Code { get; set; }
                [CustomField(101)]
                public string Name { get; set; }
            }
            """;
            yield return new((NotPartialCode, "KAONAVI001")) { DisplayName = $"{nameof(SheetDataGenerator)} > partial クラスでない場合、KAONAVI001のコンパイル警告が発生する。" };

            // lang=c#-test, strict
            const string NestedClassCode = """
            using Kaonavi.Net;
    
            public partial class Foo
            {
                [SheetSerializable]
                public partial record Bar(string Code, [property: CustomField(101)] string Name) : ISheetData;
            }
            """;
            yield return new((NestedClassCode, "KAONAVI002")) { DisplayName = $"{nameof(SheetDataGenerator)} > 入れ子にされたクラスの場合、KAONAVI002のコンパイル警告が発生する。" };

            // lang=c#-test, strict
            const string NotISheetDataCode = """
            using Kaonavi.Net;
            using Kaonavi.Net.Entities;
        
            [SheetSerializable]
            public partial record Foo(string Code, [property: CustomField(101)] string Name)
            {
                public IReadOnlyList<CustomFieldValue> ToCustomFields() => [];
            }
            """;
            yield return new((NotISheetDataCode, "KAONAVI003")) { DisplayName = $"{nameof(SheetDataGenerator)} > {nameof(ISheetData)}を継承していない場合、KAONAVI003のコンパイル警告が発生する。" };

            // lang=c#-test, strict
            const string ImplementedCode = """
            using Kaonavi.Net;
            using Kaonavi.Net.Entities;
                
            [SheetSerializable]
            public partial record Foo(string Code, [property: CustomField(101)] string Name) : ISheetData
            {
                public IReadOnlyList<CustomFieldValue> ToCustomFields() => [];
            }
            """;
            yield return new((ImplementedCode, "KAONAVI004")) { DisplayName = $"{nameof(SheetDataGenerator)} > {nameof(ISheetData.ToCustomFields)}メソッドを手動で実装している場合、KAONAVI004のコンパイル警告が発生する。" };

            // lang=c#-test, strict
            const string NoCustomFieldCode = """
            using Kaonavi.Net;
            
            [SheetSerializable]
            public partial record Foo(string Code, string Name) : ISheetData;
            """;
            yield return new((NoCustomFieldCode, "KAONAVI005")) { DisplayName = $"{nameof(SheetDataGenerator)} > {nameof(CustomFieldAttribute)}を設定したプロパティがない場合、KAONAVI005のコンパイル警告が発生する。" };

            // lang=c#-test, strict
            const string DuplicateCustomFieldCode = """
            using Kaonavi.Net;
            
            [SheetSerializable]
            public partial record Foo(string Code, [property: CustomField(101)] string Name1, [property: CustomField(101)] string Name2) : ISheetData;
            """;
            yield return new((DuplicateCustomFieldCode, "KAONAVI006")) { DisplayName = $"{nameof(SheetDataGenerator)} > {nameof(CustomFieldAttribute)}のidが重複したプロパティがある場合、KAONAVI006のコンパイル警告が発生する。" };

            // lang=c#-test, strict
            const string NoGetAccessorCode = """
            using Kaonavi.Net;
            
            [SheetSerializable]
            public partial class Foo : ISheetData
            {
                public string Code { get; set; }
                [CustomField(101)]
                public string Name { set; }
            }
            """;
            yield return new((NoGetAccessorCode, "KAONAVI007")) { DisplayName = $"{nameof(SheetDataGenerator)} > {nameof(CustomFieldAttribute)}属性を持つプロパティが get アクセサーを持っていない場合、KAONAVI007のコンパイル警告が発生する。" };
        }
    }

    /// <summary>
    /// <paramref name="code"/>をコンパイルした場合、<paramref name="id"/>のコンパイル警告が発生する。
    /// </summary>
    /// <param name="code">生成元となるソースコード</param>
    /// <param name="id">コンパイル警告のID</param>
    [TestMethod("Generator > コンパイル警告が発生する。")]
    [DynamicData(nameof(InvalidCodeTestData))]
    public void When_Invalid_Code_Compiler_Warns_With_Diagnostic(string code, string id)
    {
        // Arrange - Act
        var (_, warnings) = CSharpGeneratorRunner.RunGenerator(code);

        // Assert
        warnings.ShouldContain(d => d.Id == id);
    }

    /// <summary>
    /// <see cref="Generates_ToCustomFields_Method"/>用のテストデータ
    /// </summary>
    public static IEnumerable<TestDataRow<(LanguageVersion version, string code, string expected)>> ValidCodeTestData
    {
        get
        {
            // lang=c#-test, strict
            const string PartialClassCode = """
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
            """;
            // lang=c#-test, strict
            const string CSharp9GeneratedCode = """
            /// <auto-generated/>
            
            #nullable enable annotations
            #nullable disable warnings
            
            #pragma warning disable CS0612
            #pragma warning disable CS0618
            #pragma warning disable CS0108
            #pragma warning disable CS0162
            #pragma warning disable CS0164
            #pragma warning disable CS0219
            #pragma warning disable CS8602
            #pragma warning disable CS8619
            #pragma warning disable CS8620
            #pragma warning disable CS8631
            #pragma warning disable CA1050
            
            namespace Sample
            {
                partial class Foo
                {
                    /// <inheritdoc/>
                    public global::System.Collections.Generic.IReadOnlyList<global::Kaonavi.Net.Entities.CustomFieldValue> ToCustomFields()
                        => new[]
                        {
                            new global::Kaonavi.Net.Entities.CustomFieldValue(101, Name.ToString()),
                        };
                }
            }
            
            """;
            yield return new((LanguageVersion.CSharp9, PartialClassCode, CSharp9GeneratedCode)) { DisplayName = $"{nameof(SheetDataGenerator)} > C# 9.0 の場合、ネストされた名前空間 および 配列の初期化子を使用してソース生成する。" };

            // lang=c#-test, strict
            const string PartialRecordCode = """
            using Kaonavi.Net;
            
            namespace Sample;
                
            [SheetSerializable]
            public partial record Foo(string Code, [property: CustomField(101)] string Name) : ISheetData;
            """;
            // lang=c#-test, strict
            const string CSharp11GeneratedCode = """
            /// <auto-generated/>
            
            #nullable enable annotations
            #nullable disable warnings
            
            #pragma warning disable CS0612
            #pragma warning disable CS0618
            #pragma warning disable CS0108
            #pragma warning disable CS0162
            #pragma warning disable CS0164
            #pragma warning disable CS0219
            #pragma warning disable CS8602
            #pragma warning disable CS8619
            #pragma warning disable CS8620
            #pragma warning disable CS8631
            #pragma warning disable CA1050
            
            namespace Sample;
            
            partial record Foo
            {
                /// <inheritdoc/>
                public global::System.Collections.Generic.IReadOnlyList<global::Kaonavi.Net.Entities.CustomFieldValue> ToCustomFields()
                    => new[]
                    {
                        new global::Kaonavi.Net.Entities.CustomFieldValue(101, Name.ToString()),
                    };
            }
            
            """;
            yield return new((LanguageVersion.CSharp11, PartialRecordCode, CSharp11GeneratedCode)) { DisplayName = $"{nameof(SheetDataGenerator)} > C# 11.0 の場合、ファイルスコープ名前空間 および 配列の初期化子を使用してソース生成する。" };

            // lang=c#-test, strict
            const string CSharp12GeneratedCode = """
            /// <auto-generated/>

            #nullable enable annotations
            #nullable disable warnings

            #pragma warning disable CS0612
            #pragma warning disable CS0618
            #pragma warning disable CS0108
            #pragma warning disable CS0162
            #pragma warning disable CS0164
            #pragma warning disable CS0219
            #pragma warning disable CS8602
            #pragma warning disable CS8619
            #pragma warning disable CS8620
            #pragma warning disable CS8631
            #pragma warning disable CA1050
    
            namespace Sample;
    
            partial record Foo
            {
                /// <inheritdoc/>
                public global::System.Collections.Generic.IReadOnlyList<global::Kaonavi.Net.Entities.CustomFieldValue> ToCustomFields()
                    =>
                    [
                        new global::Kaonavi.Net.Entities.CustomFieldValue(101, Name.ToString()),
                    ];
            }

            """;
            yield return new((LanguageVersion.CSharp12, PartialRecordCode, CSharp12GeneratedCode)) { DisplayName = $"{nameof(SheetDataGenerator)} > C# 12.0 の場合、ファイルスコープ名前空間 および コレクション式を使用してソース生成する。" };
        }
    }

    /// <summary>
    /// <paramref name="version"/>を指定して<paramref name="code"/>をコンパイルした場合、<paramref name="expected"/>と同一のコードが生成される。
    /// </summary>
    /// <param name="version">C# のバージョン</param>
    /// <param name="code">生成元となるソースコード</param>
    /// <param name="expected">生成されるソースコード</param>
    [TestMethod]
    [DynamicData(nameof(ValidCodeTestData))]
    public void Generates_ToCustomFields_Method(LanguageVersion version, string code, string expected)
    {
        // Arrange - Act
        var (compilation, warnings) = CSharpGeneratorRunner.RunGenerator(code, version);
        var sb = new StringBuilder();
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            if (!syntaxTree.FilePath.EndsWith(".g.cs"))
                continue;
            sb.Append(syntaxTree.ToString());
        }

        // Assert
        warnings.ShouldBeEmpty();
        sb.ToString().ShouldBe(expected, StringCompareShould.IgnoreLineEndings);
    }
}
