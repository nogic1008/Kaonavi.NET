using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Kaonavi.Net.Tests;

/// <summary><see cref="Generator.SheetDataGenerator"/>の単体テスト</summary>
[TestClass, TestCategory("Source Generator")]
public sealed class SheetDataGeneratorTest
{
    [TestMethod]
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
        var (compilation, diagnostics) = CSharpGeneratorRunner.RunGenerator(code);
        var compilationDiagnostics = compilation.GetDiagnostics();
        var warnings = diagnostics.Concat(compilationDiagnostics).Where(x => x.Severity >= DiagnosticSeverity.Warning).ToArray();

        warnings.Select(x => x.Id).Should().Contain(id);
    }
}
