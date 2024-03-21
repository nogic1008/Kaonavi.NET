using ConsoleAppSample;
using Kaonavi.Net.Services;
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
    services.AddHttpClient<IKaonaviService, KaonaviV2Service>((client, provider) =>
    {
        var options = provider.GetRequiredService<IOptions<KaonaviOptions>>().Value;
        return new(client, options.ConsumerKey, options.ConsumerSecret)
        {
            UseDryRun = options.UseDryRun
        };
    });
});
var app = builder.Build();

app.AddCommand("layout", "メンバー情報のレイアウトを取得します。", async (ConsoleAppContext ctx, IKaonaviService service) =>
{
    var memberLayout = await service.FetchMemberLayoutAsync(ctx.CancellationToken).ConfigureAwait(false);
    ctx.Logger.LogInformation("Received Layout: {memberLayout}", memberLayout);
});
app.AddCommand("download", "メンバー情報を全取得します。", async (ConsoleAppContext ctx, IKaonaviService service) =>
{
    var employees = (await service.Member.ListAsync(ctx.CancellationToken).ConfigureAwait(false))
        .Select(m => new EmployeeData(m));
    ctx.Logger.LogInformation("Received Data: {employees}", employees);
});
app.AddCommand("upload", "メンバー情報を更新します。", async (ConsoleAppContext ctx, IKaonaviService service) =>
{
    var employees = Enumerable.Range(1, 9)
        .Select(i => new EmployeeData($"100{i}", $"User {i}", $"User {i}", $"{i}000", $"100{i}@example.com", "男", new(1990, 1, 1), "A", new(2012, 4, 1)));
    int taskId = await service.Member.UpdateAsync(employees.Select(e => e.ToMemberData()).ToArray(), ctx.CancellationToken).ConfigureAwait(false);
    ctx.Logger.LogInformation("Start task at (TaskId: {taskId})", taskId);
});
app.AddCommand("progress", "タスクの進捗状況を取得します。", async (ConsoleAppContext ctx, IKaonaviService service, [Option("t", "タスクID")] int taskId) =>
{
    var progress = await service.FetchTaskProgressAsync(taskId, ctx.CancellationToken).ConfigureAwait(false);
    ctx.Logger.LogInformation("Received Progress: {progress}", progress);
});
