using ConsoleAppSample;
using Kaonavi.Net;
using Kaonavi.Net.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx, services) =>
{
    // IOptions
    var config = ctx.Configuration;
    services.Configure<KaonaviOptions>(config.GetSection(nameof(KaonaviOptions)));

    // DI
    services.AddHttpClient<IKaonaviClient, KaonaviClient>((client, provider) =>
    {
        var options = provider.GetRequiredService<IOptions<KaonaviOptions>>().Value;
        return new(client, options.ConsumerKey, options.ConsumerSecret)
        {
            UseDryRun = options.UseDryRun
        };
    });
});
var app = builder.Build();

app.AddCommand("layout", "メンバー情報のレイアウトを取得します。", async (ConsoleAppContext ctx, IKaonaviClient client) =>
{
    var memberLayout = await client.Layout.ReadMemberLayoutAsync(ctx.CancellationToken).ConfigureAwait(false);
    ctx.Logger.LogInformation("Received Layout: {memberLayout}", memberLayout);
});
app.AddCommand("download", "メンバー情報を全取得します。", async (ConsoleAppContext ctx, IKaonaviClient client) =>
{
    var employees = (await client.Member.ListAsync(ctx.CancellationToken).ConfigureAwait(false))
        .Select(m => new EmployeeData(m));
    ctx.Logger.LogInformation("Received Data: {employees}", employees);
});
app.AddCommand("upload", "メンバー情報を更新します。", async (ConsoleAppContext ctx, IKaonaviClient client) =>
{
    var employees = Enumerable.Range(1, 9)
        .Select(i => new EmployeeData($"100{i}", $"User {i}", $"User {i}", $"{i}000", $"100{i}@example.com", "男", new(1990, 1, 1), "A", new(2012, 4, 1)));
    var customSheets = Enumerable.Range(1, 9)
        .Select(i => new CustomSheetData($"100{i}", $"100-000{i}", $"Address {i}", new(2023, 1, i)));
    int taskId1 = await client.Member.UpdateAsync(employees.Select(e => e.ToMemberData()).ToArray(), ctx.CancellationToken).ConfigureAwait(false);
    ctx.Logger.LogInformation("Start task at (TaskId: {taskId})", taskId1);
    int taskId2 = await client.Sheet.UpdateAsync(1, customSheets.ToSingleSheetData(), ctx.CancellationToken).ConfigureAwait(false);
    ctx.Logger.LogInformation("Start task at (TaskId: {taskId})", taskId2);
});
app.AddCommand("progress", "タスクの進捗状況を取得します。", async (ConsoleAppContext ctx, IKaonaviClient client, [Option("t", "タスクID")] int taskId) =>
{
    var progress = await client.Task.ReadAsync(taskId, ctx.CancellationToken).ConfigureAwait(false);
    ctx.Logger.LogInformation("Received Progress: {progress}", progress);
});
