# KAONAVI001: `SheetSerializable` object should be partial

| プロパティ              | 値                                                             |
|------------------------|----------------------------------------------------------------|
| **Rule ID**            | KAONAVI001                                                     |
| **Title**              | `SheetSerializable` オブジェクトは `partial` でなければなりません |
| **Category**           | KaonaviSourceGenerate                                          |
| **Enabled by default** | Yes                                                            |

## Causes

[`SheetSerializable`](../../src/Kaonavi.NET.Core/SheetSerializableAttribute.cs)属性の付与されたクラス(またはレコード クラス)が、[部分クラス](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods)として定義されていません。

## Rule Description

`SheetSerializable`属性のついたクラスに対して、ソース ジェネレーターは、`ISheetData`インターフェイスの実装を[部分クラス](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods)として生成しようとします。  
部分クラスの定義を含むクラスは、すべて`partial`キーワードを持つ必要があり、この制約に違反した場合、
[CS0260](https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-messages/cs0260)コンパイル エラーが発生します。

このコンパイル エラーを回避するため、ソース ジェネレーターはソース生成を行う代わりに、この警告を発生させます。

## How to Fix Violations

`SheetSerializable`属性を付けたクラスを部分クラスとして定義します。([例](#example)を参照)

もしくは、`SheetSerializable`属性を削除して、ソース生成を行わないよう明示してください。

## Example

```csharp
using Kaonavi.Net;

[SheetSerializable]
public /* partial */ class NGSample : ISheetData // KAONAVI001
{
    public string Code { get; set; }
    [CustomField(1)]
    public string Name { get; set; }
}
// Source generator does not generate ToCustomFields(), so it also causes CS0535 error.

[SheetSerializable]
public partial class OKSample : ISheetData // OK
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
