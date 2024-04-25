# KAONAVI002: `SheetSerializable` object should not be inner class

| プロパティ              | 値                                                     |
|------------------------|--------------------------------------------------------|
| **Rule ID**            | KAONAVI002                                             |
| **Title**              | `SheetSerializable` オブジェクトは 内部クラスにできません |
| **Category**           | KaonaviSourceGenerate                                  |
| **Enabled by default** | Yes                                                    |

## Causes

[`SheetSerializable`](../../src/Kaonavi.NET.Core/SheetSerializableAttribute.cs) 属性の付与されたクラス(またはレコード クラス)が、[入れ子にされた型](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/nested-types)として定義されています。

## Rule Description

`SheetSerializable` 属性のついたクラスに対して、ソース ジェネレーターは、`ISheetData` インターフェイスの実装を別ファイルの[部分クラス](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods)として生成しようとします。  
クラスの定義を別ファイルに分割する場合は、**外側のクラスも含めて**すべて`partial`キーワードを持つ必要があり、この制約に違反した場合、
[CS0260](https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-messages/cs0260)コンパイル エラーが発生します。

入れ子にされた型に対して部分クラスを適用可能かどうかを判断するロジックは煩雑です。そのため、ソース ジェネレーターはソース生成を行う代わりに、この警告を発生させます。

## How to Fix Violations

`SheetSerializable` 属性を付けたクラスをトップ クラスとして定義します。([例](#example)を参照)

もしくは、`SheetSerializable` 属性を削除して、ソース生成を行わないよう明示してください。

## Example

```csharp
using Kaonavi.Net;
using Kaonavi.Net.Entities;

public class OuterClass1 // Missing partial
{
    [SheetSerializable]
    public partial class NGClass : ISheetData // KAONAVI002
    {
        public string Code { get; set; }
        [CustomField(1)]
        public string Name { get; set; }
    }
    // Source generator does not generate ToCustomFields(), so it also causes CS0535 error.
}

public partial class OuterClass2
{
    // In reality, source generator can define partial class, but it don't.
    [SheetSerializable]
    public partial class NGClass : ISheetData // KAONAVI002
    {
        public string Code { get; set; }
        [CustomField(1)]
        public string Name { get; set; }
    }
    // Source generator does not generate ToCustomFields(), so it also causes CS0535 error.
}

[SheetSerializable]
public partial class OKSample : ISheetData
{
    public string Code { get; set; }
    [CustomField(1)]
    public string Name { get; set; }
}
// Source genaretor genarates such as:
partial class OKSample
{
    public IReadOnlyList<CustomFieldValue> ToCustomFields() => ...
}
```
