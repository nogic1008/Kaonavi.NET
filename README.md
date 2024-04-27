# Kaonavi.NET

[![NuGet (stable version)](https://img.shields.io/nuget/v/Kaonavi.NET?logo=nuget)](https://www.nuget.org/packages/Kaonavi.NET/)
[![GitHub releases (including pre-releases)](https://img.shields.io/github/v/release/nogic1008/Kaonavi.NET?include_prereleases&sort=semver)](https://github.com/nogic1008/Kaonavi.NET/releases)
[![.NET CI](https://github.com/nogic1008/Kaonavi.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/nogic1008/Kaonavi.NET/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/nogic1008/Kaonavi.NET/branch/main/graph/badge.svg?token=DK9S9TJtgj)](https://codecov.io/gh/nogic1008/Kaonavi.NET)
[![CodeFactor](https://www.codefactor.io/repository/github/nogic1008/Kaonavi.NET/badge)](https://www.codefactor.io/repository/github/nogic1008/Kaonavi.NET)
[![License](https://img.shields.io/github/license/nogic1008/Kaonavi.NET)](https://github.com/nogic1008/Kaonavi.NET/blob/main/LICENSE)

Unofficial Kaonavi Library for .NET

> [!WARNING]
> 現状、メンテナーがカオナビAPIの利用権を持っていないため、実際の動作確認ができていません。
> ご利用の際は自己責任でお願いいたします。

## Install

```powershell
# Package Manager
> Install-Package Kaonavi.NET

# .NET CLI
> dotnet add package Kaonavi.NET
```

## Usage

事前に[公式APIドキュメント](https://developer.kaonavi.jp/api/v2.0/index.html#section/%E3%82%AF%E3%82%A4%E3%83%83%E3%82%AF%E3%82%B9%E3%82%BF%E3%83%BC%E3%83%88/%E4%BA%8B%E5%89%8D%E6%BA%96%E5%82%99)の手順に従い、Consumer KeyとConsumer Secretを取得してください。

### Basic

```csharp
using Kaonavi.Net;
using Kaonavi.Net.Entities;

var client = new KaonaviClient(new HttpClient(), "Your Consumer Key", "Your Consumer Secret");

// アクセストークンは最初にAPIを呼び出す際に自動で取得されます

// 所属ツリー 一括取得APIを呼び出す
var departments = await client.Department.ListAsync();

// メンバー情報 登録APIを呼び出す(戻り値はタスクID)
int taskId = await client.Member.CreateAsync(new MemberData[]
{
    new MemberData(
        Code: "A0002",
        Name: "カオナビ 太郎",
        NameKana: "カオナビ タロウ",
        Mail: "taro@kaonavi.jp",
        EnteredDate: new DateOnly(2005, 9, 20),
        RetiredDate: null,
        Gender: "男性",
        Birthday: new DateOnly(1984, 5, 15),
        Department: new MemberDepartment("1000"),
        SubDepartments: new MemberDepartment[] { new MemberDepartment("1001") },
        CustomFields: new CustomFieldValue[] { new CustomFieldValue(100, "A") }
    ),
});

// 上記APIがサーバー側で処理されるまで待つ
await Task.Delay(10000);

// タスク進捗状況 取得APIを呼び出す
var progress = await client.Task.ReadAsync(taskId);
if (progress.Status == "NG" || progress.Status == "ERROR")
{
    // エラー処理
}
```

### Dependency Injection

クライアントは`IKaonaviClient`で抽象化されているため、DIコンテナを使用して登録することができます。

> [!TIP]
> コンソール アプリの完全なサンプルは[ConsoleAppSample](https://github.com/nogic1008/Kaonavi.NET/tree/main/sandbox/ConsoleAppSample)を参照してください。

```csharp
// Microsoft.Extensions.DependencyInjectionでの使用例
hostBuilder.ConfigureServices((context, services) =>
{
    string consumerKey = context.Configuration["Kaonavi:ConsumerKey"];
    string consumerSecret = context.Configuration["Kaonavi:ConsumerSecret"];
    // HttpClientを依存性注入しつつ、KaonaviClientをDIに登録
    // 要 Microsoft.Extensions.Http パッケージ
    services.AddHttpClient<IKaonaviClient, KaonaviClient>((client) => new(client, consumerKey, consumerSecret));

    services.addTransient<IYourService, YourService>();
});

public interface IYourService
{
    public async Task DoSomethingAsync();
}

// カオナビAPIを呼び出すサービス
public class YourService : IYourService
{
    private readonly IKaonaviClient _client;

    // コンストラクターで依存性を注入
    public YourService(IKaonaviClient client) => _client = client;

    public async Task DoSomethingAsync()
    {
        var departments = await _client.Department.ListAsync();
        // ...
    }
}
```

### Source Generator

Kaonavi.NETでは、独自のクラスからシート情報への生成を簡単にするためのソース ジェネレーターを提供しています。

> [!IMPORTANT]
> Visual Studio 2022 (バージョン 17.3)以降、もしくは Visual Studio Code の C# 拡張機能などの`Microsoft.CodeAnalysis.CSharp` 4.3.0以降に対応したエディターが必要です。

```csharp
using Kaonavi.Net;
using Kaonavi.Net.Entities;

// 1. SheetSerializable属性を付与したpartialクラス(recordクラスも可)を定義
[SheetSerializable]
public partial class Position : ISheetData // 2. ISheetDataを実装
{
    public string Code { get; set; } // 3. ISheetData.Codeプロパティを実装
    [CustomField(100)] // 4. カスタムフィールドとなるプロパティにCustomField属性を付与
    public string Name { get; set; }

    // 以下のメソッドが自動生成される
    public IReadOnlyList<CustomFieldValue> ToCustomFields() => new CustomFieldValue[]
    {
        new CustomFieldValue(100, Name),
    };
}

var positions = new Position[]
{
    new Position { Code = "A0001", Name = "社長" },
    new Position { Code = "A0002", Name = "部長" },
};

var client = new KaonaviClient(new HttpClient(), "Your Consumer Key", "Your Consumer Secret");

// ISheetDataを実装することで、シート情報に変換する拡張メソッドが利用可能
// IEnumerable<ISheetData>.ToSingleSheetData(): 単一レコードのシート情報に変換
// IEnumerable<ISheetData>.ToMultipleSheetData(): 複数レコードのシート情報に変換
var sheetDataList = positions.ToSingleSheetData();
// シート情報 一括更新APIを呼び出す
var taskId = await client.Sheet.ReplaceAsync(i, sheetDataList);
```

## Development & Contributing

[CONTRIBUTING.md](https://github.com/nogic1008/Kaonavi.NET/blob/main/CONTRIBUTING.md)を参照してください。
