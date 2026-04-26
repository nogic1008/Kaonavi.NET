---
applyTo: "src/Kaonavi.NET.Core/Entities/**"
---

# エンティティ設計規約

`src/Kaonavi.NET.Core/Entities/` に適用されるルールです。

---

## エンティティの設計方針

### `record class` を使う

エンティティはすべて **`record class`** で定義する。`record struct` と通常の `class` は使わない。

```csharp
// 良い例
public record MemberData(
    string Code,
    string? Name = null,
    IReadOnlyList<CustomFieldValue>? CustomFields = null
);

// 悪い例（class は使わない）
public class MemberData
{
    public string Code { get; init; } = "";
}
```

### Primary Constructor を必須にする

`record class` は必ず primary constructor 形式で定義する。プロパティを個別宣言する形式は採用しない。

```csharp
// 良い例
public record DepartmentTree(string Code, string Name, string? ParentCode = null);

// 悪い例（primary constructor を使わない）
public record DepartmentTree
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
```

### プロパティ

- プロパティは **`init`** アクセサのみ（ミュータブルな `set` は使わない）。
- コレクションは その振る舞いに着目して、適したインターフェース型を使う（`List<T>` / `T[]` などの具象型は返さない）。
  - **`IReadOnlyList<T>`** : 順序が意味を持ち、インデックスアクセスが必要な場合。
  - **`IReadOnlyCollection<T>`** : 順序が意味を持たない、単純な列挙だけで十分な場合。
- null 許容は `?` で明示する。null 非許容プロパティには有効な値を必ず渡す。

### コンストラクター引数の順序

1. 識別子（例: `Code`, `Id`）
2. 必須プロパティ（null 非許容）
3. 任意プロパティ（null 許容 or デフォルト値あり）

```csharp
public record DepartmentTree(
    string Code,        // 識別子
    string Name,        // 必須
    string? ParentCode, // 任意（null 許容）
    int Order = 0,      // 任意（デフォルトあり）
    string? Memo = null // 任意（デフォルト null）
);
```

### XML ドキュメントコメント

すべての `public record` と各パラメータに XML ドキュメントコメントを付ける：

```csharp
/// <summary>メンバー情報（基本情報/所属(主務)/兼務情報）</summary>
/// <param name="Code">社員番号</param>
/// <param name="Name">氏名</param>
public record MemberData(string Code, string? Name = null);
```

---

## JSON シリアライゼーション属性

- JSON プロパティ名は標準の snake_case ポリシーに従う。特別な理由がない限り `[JsonPropertyName]` は付けない。
  - 元のJSONと異なるプロパティ名が必要な場合のみ `[JsonPropertyName("...")]` を明示する。
- カスタム変換が必要な場合は `[property: JsonConverter(typeof(SomeConverter))]` をパラメータに付ける。現状では、以下のConverterを定義している。
  - `BlankNullableDateConverter`: JSONの空文字列を `null` として扱う `DateOnly?`
  - `DateTimeConverter`: `DateTime` のカスタムフォーマット(`yyyy-MM-dd HH:mm:ss`)
  - `EnumOptionPayloadConverter`: `{ id: 1, name: "name" }`をタプルに変換

```csharp
public record MemberData(
    string Code,
    [property: JsonConverter(typeof(BlankNullableDateConverter))] DateOnly? EnteredDate = default
);
```

---

## テスト

エンティティを追加・変更した場合は、対応する JSON デシリアライズテストを必ず追加または更新する。
対象テストは `test/Kaonavi.NET.Core.Tests/Entities/` 配下に配置する。

---

## ペイロード（送信用）とレスポンス（受信用）の命名

| 種別 | サフィックス | 例 |
|---|---|---|
| API レスポンスの受信 | なし | `MemberData`, `SheetData` |
| API リクエストの送信 | `Payload` | `UserPayload`, `AttachmentPayload` |
| レイアウト情報 | `Layout` | `MemberLayout`, `SheetLayout` |
| フィールド情報 | `FieldLayout`, `FieldType` | `CustomFieldLayout` |
