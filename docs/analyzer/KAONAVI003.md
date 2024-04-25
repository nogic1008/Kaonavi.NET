# KAONAVI003: `SheetSerializable` object should implement `ISheetData`

| プロパティ              | 値                                                                   |
|------------------------|----------------------------------------------------------------------|
| **Rule ID**            | KAONAVI003                                                           |
| **Title**              | `SheetSerializable` オブジェクトは `ISheetData` を実装する必要があります |
| **Category**           | KaonaviSourceGenerate                                                |
| **Enabled by default** | Yes                                                                  |

## Causes

[`SheetSerializable`](../../src/Kaonavi.NET.Core/SheetSerializableAttribute.cs)属性の付与されたクラス(またはレコード クラス)が、[ISheetData](../../src/Kaonavi.NET.Core/ISheetData.cs)インターフェースを実装していません。

## Rule Description

`SheetSerializable`属性を付与することは、ソース ジェネレーターに`ISheetData.ToCustomFields()`の実装を移譲することを意味します。  
ソース ジェネレーターは、`ISheetData.ToCustomFields()`を[暗黙的に実装](https://learn.microsoft.com/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation)します。そのため`ISheetData`がない場合、コンパイル エラーは発生しませんが、`ToCustomFields()`はクラスのメンバーとして扱われます。

この挙動はユーザーの意図したものでない可能性が高いため、ソース ジェネレーターはソース生成を行う代わりに、この警告を発生させます。

## How to Fix Violations

`SheetSerializable` 属性を付けたクラスで`ISheetData`インターフェースを実装します。([例](#example)を参照)

もしくは、`SheetSerializable` 属性を削除して、ソース生成を行わないよう明示してください。

## Example

```csharp
using Kaonavi.Net;

[SheetSerializable]
public partial class NGSample/* : ISheetData */ // KAONAVI003
{
    [CustomField(1)]
    public string Name { get; set; }
}
// Source generator does not generate ToCustomFields()

[SheetSerializable]
public partial class OKSample : ISheetData
{
    public string Code { get; set; } // Needs to implement ISheetData
    [CustomField(1)]
    public string Name { get; set; }
}
// Source genaretor genarates such as:
partial class OKSample
{
    public IReadOnlyList<CustomFieldValue> ToCustomFields() => ...
}
```
