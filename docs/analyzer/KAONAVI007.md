# KAONAVI006: `CustomField` should have getter

| プロパティ              | 値                                        |
|------------------------|--------------------------------------------|
| **Rule ID**            | KAONAVI007                                 |
| **Title**              | `CustomField`には`get`アクセサーが必要です |
| **Category**           | KaonaviSourceGenerate                      |
| **Enabled by default** | Yes                                        |

## Causes

[`SheetSerializable`](../../src/Kaonavi.NET.Core/SheetSerializableAttribute.cs)属性を持つクラス(またはレコード クラス)内に、`get`アクセサーのない[`CustomField`](../../src/Kaonavi.NET.Core/CustomFieldAttribute.cs)属性を持つプロパティが存在します。

## Rule Description

`SheetSerializable`属性を持つクラスに対して、ソース ジェネレーターは、`CustomField`属性のついたプロパティから値を読み取る`ToCustomFields`メソッドを生成しようとします。  
しかし、`CustomField`属性のついたプロパティに`get`アクセサーがない場合、そのプロパティの値を取得できないため、`ToCustomFields`メソッド内でコンパイル エラーが発生します。

このコンパイル エラーを回避するため、ソース ジェネレーターはソース生成を行う代わりに、この警告を発生させます。

## How to Fix Violations

`CustomField`のついたプロパティに`get`アクセサーを追加します。([例](#example)を参照)

もしくは、`SheetSerializable`属性を削除して、ソース生成を行わないよう明示してください。

## Example

```csharp
using Kaonavi.Net;

[SheetSerializable]
public partial class NGSample : ISheetData
{
    public string Code { get; set; }
    [CustomField(1)] // KAONAVI007
    public string Name1 { set; }
}
// Source generator does not generate ToCustomFields(), so it also causes CS0535 error.

[SheetSerializable]
public partial class OKSample : ISheetData
{
    public string Code { get; set; }
    [CustomField(1)]
    public string Name1 { get; set; }
}
// Source genaretor genarates such as:
partial class OKSample
{
    public IReadOnlyList<CustomFieldValue> ToCustomFields() => ...
}
```

