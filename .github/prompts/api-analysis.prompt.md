---
description: "カオナビ API v2 ドキュメントと現在の実装を比較し、未実装・不足しているエンドポイントを列挙する"
argument-hint: "調査対象のカテゴリ（例: member, sheet, department）。省略すると全カテゴリを対象にする"
agent: "agent"
tools: [fetch, codebase, search]
---

カオナビ API v2 の公式ドキュメント（https://developer.kaonavi.jp/api/v2.0/index.html）を取得し、現在の実装（`src/Kaonavi.NET.Core/`）と照らし合わせて、**未実装・不足しているエンドポイントや機能**を列挙してください。

## 調査手順

1. **ドキュメント取得**: 上記 URL からカオナビ API v2 のエンドポイント一覧を取得する。引数でカテゴリが指定された場合はそのカテゴリのみを対象にする。
2. **実装確認**: `IKaonaviClient.cs` および `KaonaviClient.*.cs` を調べ、各エンドポイントに対応するメソッドが存在するかを確認する。
3. **差分レポート**: 以下の形式で結果を出力する。

## 出力形式

### 未実装のエンドポイント

| HTTP メソッド | パス | 説明 | 対応するインターフェース |
|---|---|---|---|
| POST | `/members` | メンバー追加 | `IMember.CreateAsync()` |

### 実装済みだが仕様と異なる可能性があるもの

- （パラメータの過不足・型の不一致など気になる点があれば記載）

### 備考

- 調査対象外にしたエンドポイント（廃止済み・β版など）があれば注記する。

---

> **参照ドキュメント**:
> - 命名規則: `AGENTS.md` の「APIメソッドの命名規則」セクション
> - 実装パターン（インターフェース設計・カテゴリ追加手順など）: [client-implementation.instructions.md](../instructions/client-implementation.instructions.md)
