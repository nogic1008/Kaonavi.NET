# KAONAVI006: `CustomField` should be unique

| プロパティ              | 値                                       |
|------------------------|------------------------------------------|
| **Rule ID**            | KAONAVI006                               |
| **Title**              | `CustomField`は一意の値でなければなりません |
| **Category**           | KaonaviSourceGenerate                    |
| **Enabled by default** | Yes                                      |

## Causes

[`SheetSerializable`](../../src/Kaonavi.NET.Core/SheetSerializableAttribute.cs)属性を持つクラス(またはレコード クラス)内に、`id`の同じ[`CustomField`](../../src/Kaonavi.NET.Core/CustomFieldAttribute.cs)属性を持つプロパティが複数存在します。

## Rule Description

`SheetSerializable`属性を持つクラスに対して、ソース ジェネレーターは、`CustomField`属性の`id`の値をキーとした[`CustomFieldValue`](../../src/Kaonavi.NET.Core/Entities/CustomFieldValue.cs)のリストを生成しようとします。  
`CustomFieldValue`のIDが一意でない場合、APIリクエストのBodyパラメータが正しく生成されないため、API呼び出しが失敗します。

この挙動はユーザーの意図したものでない可能性が高いため、ソース ジェネレーターはソース生成を行う代わりに、この警告を発生させます。

## How to Fix Violations

`CustomField`属性の`id`値を一意にするように修正してください。([例](#example)を参照)

もしくは、`SheetSerializable`属性を削除して、ソース生成を行わないよう明示してください。

## Example

```csharp
using Kaonavi.Net;

[SheetSerializable]
public partial class NGSample : ISheetData
{
    public string Code { get; set; }
    [CustomField(1)] // KAONAVI006
    public string Name1 { get; set; }
    [CustomField(1)] // KAONAVI006
    public string Name2 { get; set; }
}
// Source generator does not generate ToCustomFields(), so it also causes CS0535 error.

[SheetSerializable]
public partial class OKSample : ISheetData
{
    public string Code { get; set; }
    [CustomField(1)]
    public string Name1 { get; set; }
    [CustomField(2)]
    public string Name2 { get; set; }
}
// Source genaretor genarates such as:
partial class OKSample
{
    public IReadOnlyList<CustomFieldValue> ToCustomFields() => ...
}
```

