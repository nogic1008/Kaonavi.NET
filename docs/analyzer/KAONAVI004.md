# KAONAVI004: `SheetSerializable` object should not implement `ToCustomFields()`

| プロパティ              | 値                                                                       |
|------------------------|--------------------------------------------------------------------------|
| **Rule ID**            | KAONAVI004                                                               |
| **Title**              | `SheetSerializable` オブジェクトは `ToCustomFields()` を実装してはいけません |
| **Category**           | KaonaviSourceGenerate                                                    |
| **Enabled by default** | Yes                                                                      |

## Causes

[`SheetSerializable`](../../src/Kaonavi.NET.Core/SheetSerializableAttribute.cs)属性の付与されたクラス(またはレコード クラス)が、`ToCustomFields()`メソッドを実装しています。

## Rule Description

`SheetSerializable`属性を付与することは、ソース ジェネレーターに`ISheetData.ToCustomFields()`の実装を移譲することを意味します。  
ソース ジェネレーターは、`ISheetData.ToCustomFields()`を[暗黙的に実装](https://learn.microsoft.com/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation)します。そのため`ToCustomFields()`がすでに実装されている場合、実装が競合することにより[CS0102](https://learn.microsoft.com/dotnet/csharp/misc/cs0102)コンパイル エラーが発生します。

このコンパイル エラーを回避するため、ソース ジェネレーターはソース生成を行う代わりに、この警告を発生させます。

## How to Fix Violations

`SheetSerializable`属性を付けたクラスから`ToCustomFields()`メソッドを削除してください。([例](#example)を参照)

もしくは、`SheetSerializable`属性を削除して、ソース生成を行わないよう明示してください。

## Example

```csharp
using Kaonavi.Net;
using Kaonavi.Net.Entities;

[SheetSerializable]
public partial class NGSample : ISheetData
{
    public string Code { get; set; }
    [CustomField(1)]
    public string Name { get; set; }
    public IReadOnlyList<CustomFieldValue> ToCustomFields() => ... // KAONAVI004
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
