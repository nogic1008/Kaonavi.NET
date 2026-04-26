---
applyTo: "src/Kaonavi.NET.Core/PublicAPI.*.txt"
---

# Public API 管理規約

`src/Kaonavi.NET.Core/PublicAPI.*.txt` に適用されるルールです。

---

## 管理方針

- Public API は `Microsoft.CodeAnalysis.PublicApiAnalyzers` の診断に従って管理する。
- 手動でシグネチャを列挙・推測して更新しない。
- `dotnet build` を実行し、Analyzer が要求した差分のみ `PublicAPI.Unshipped.txt` に反映する。

---

## ファイルの役割

| ファイル | 内容 |
|---|---|
| `PublicAPI.Shipped.txt` | リリース済みの確定 API シグネチャ |
| `PublicAPI.Unshipped.txt` | 次のリリースで追加される API シグネチャ |

---

## 変更時のルール

- 新しい `public` 型・メンバーを追加したときは、Analyzer 診断に基づき `PublicAPI.Unshipped.txt` を更新する。
- `record class` の自動生成メンバー（`Equals` / `GetHashCode` / `ToString` / `op_Equality` / `op_Inequality` など）も Analyzer が要求する場合は反映する。
- `PublicAPI.Shipped.txt` の既存シグネチャは変更・削除しない。必要な変更は新規 API 追加で対応し、破壊的変更はメジャーバージョン更新時のみ許容する。

---

## 完了条件

- `dotnet build` 実行後、Public API Analyzer の警告が 0 であること。
