# KAONAVI005: `SheetSerializable` object should have property with `CustomField`

| プロパティ              | 値                                                                            |
|------------------------|-------------------------------------------------------------------------------|
| **Rule ID**            | KAONAVI005                                                                    |
| **Title**              | `SheetSerializable` オブジェクトには `CustomField` 属性を持つプロパティが必要です |
| **Category**           | KaonaviSourceGenerate                                                         |
| **Enabled by default** | Yes                                                                           |

## Causes

[`SheetSerializable`](../../src/Kaonavi.NET.Core/SheetSerializableAttribute.cs) 属性を持つクラス(またはレコード クラス)に、[`CustomField`](../../src/Kaonavi.NET.Core/CustomFieldAttribute.cs)属性を持つプロパティが存在しません。

## Rule Description

`SheetSerializable`属性を持つクラスに対して、ソース ジェネレーターは、`CustomField`属性の付与されたプロパティから[`CustomFieldValue`](../../src/Kaonavi.NET.Core/Entities/CustomFieldValue.cs)のリストを生成しようとします。  
そのため、`CustomField`属性を持つプロパティが存在しない場合、`ToCustomFields()`メソッドは常に空のリストを返します。

この挙動はユーザーの意図したものでない可能性が高いため、ソース ジェネレーターはソース生成を行う代わりに、この警告を発生させます。

## How to Fix Violations

`SheetSerializable`属性を付けたクラスに、`CustomField`属性を持つプロパティを追加します。([例](#example)を参照)

もしくは、`SheetSerializable`属性を削除して、ソース生成を行わないよう明示してください。

## Example

```csharp
using Kaonavi.Net;

[SheetSerializable]
public partial class NGSample : ISheetData // KAONAVI005
{
    public string Code { get; set; }
    /* [CustomField(1)] */
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
