using ConsoleAppFramework;
using ConsoleAppSample;
using ConsoleAppSample.Entities;
using Kaonavi.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var app = ConsoleApp.Create()
    .ConfigureDefaultConfiguration()
    .ConfigureLogging(static services => services.AddConsole())
    .ConfigureServices(static (configuration, services) =>
    {
        // IOptions
        services.Configure<KaonaviOptions>(configuration.GetSection(nameof(KaonaviOptions)));

        // IKaonaviClient
        services.AddHttpClient<IKaonaviClient, KaonaviClient>((client, provider) =>
        {
            var options = provider.GetRequiredService<IOptions<KaonaviOptions>>().Value;
            return new(client, options.ConsumerKey, options.ConsumerSecret)
            {
                UseDryRun = options.UseDryRun
            };
        });
    });
app.Add<Commands>();
app.Run(args);


/// <summary>
/// カオナビのAPIを操作するコマンド群
/// </summary>
/// <param name="logger">ログ記録用インスタンス (DI)</param>
/// <param name="client">カオナビAPIを呼び出すクライアント (DI)</param>
internal class Commands(ILogger<Commands> logger, IKaonaviClient client)
{
    /// <summary>メンバー情報のレイアウトを取得します。</summary>
    public async Task Layout(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var memberLayout = await client.Layout.ReadMemberLayoutAsync(false, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Received Layout: {memberLayout}", memberLayout);
    }

    /// <summary>メンバー情報を全取得します。</summary>
    public async Task Download(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var employees = (await client.Member.ListAsync(cancellationToken).ConfigureAwait(false))
            .Select(m => new EmployeeData(m));
        logger.LogInformation("Received Data: {employees}", employees);
    }

    /// <summary>メンバー情報を更新します。</summary>
    public async Task Upload(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var employees = Enumerable.Range(1, 9)
            .Select(i => new EmployeeData($"100{i}", $"User {i}", $"User {i}", $"{i}000", $"100{i}@example.com", "男", new(1990, 1, 1), "A", new(2012, 4, 1)));
        var customSheets = Enumerable.Range(1, 9)
            .Select(i => new CustomSheetData($"100{i}", $"100-000{i}", $"Address {i}", new(2023, 1, i)));
        int taskId1 = await client.Member.UpdateAsync([.. employees.Select(e => e.ToMemberData())], cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Start task at (TaskId: {taskId})", taskId1);
        int taskId2 = await client.Sheet.UpdateAsync(1, customSheets.ToSingleSheetData(), cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Start task at (TaskId: {taskId})", taskId2);
    }

    /// <summary>タスクの進捗状況を取得します。</summary>
    /// <param name="taskId">-t, タスクID</param>
    public async Task Progress(int taskId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var progress = await client.Task.ReadAsync(taskId, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Received Progress: {progress}", progress);
    }
}
