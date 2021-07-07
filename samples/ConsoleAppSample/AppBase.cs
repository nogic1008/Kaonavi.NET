using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Kaonavi.Net.Services;
using Microsoft.Extensions.Logging;

namespace ConsoleAppSample
{
    /// <summary>
    /// アプリケーションのメインロジック クラス。
    /// </summary>
    public class AppBase : ConsoleAppBase
    {
        /// <summary>カオナビAPIサービス(DI)</summary>
        private readonly IKaonaviService _kaonaviService;

        /// <summary>
        /// AppBaseの新しいインスタンスを生成します。
        /// .NET Generic Hostより呼び出されます。
        /// </summary>
        /// <param name="kaonaviService">カオナビAPIサービス(DI)</param>
        public AppBase(IKaonaviService kaonaviService) => _kaonaviService = kaonaviService;

        /// <summary>
        /// メンバー情報のレイアウトを取得します。
        /// </summary>
        [Command("layout", "メンバー情報のレイアウトを取得します。")]
        public async ValueTask FetchLayoutAsync()
        {
            var memberLayout = await _kaonaviService.FetchMemberLayoutAsync(Context.CancellationToken).ConfigureAwait(false);
            Context.Logger.LogInformation("Received Layout: {0}", JsonSerializer.Serialize(memberLayout));
        }

        /// <summary>
        /// メンバー情報を全取得します。
        /// </summary>
        [Command("download", "メンバー情報を全取得します。")]
        public async ValueTask DownloadEmployeeAsync()
        {
            var employees = (await _kaonaviService.FetchMembersDataAsync(Context.CancellationToken).ConfigureAwait(false))
                .Select(m => new EmployeeData(m));
            Context.Logger.LogInformation("Received Data: {employees}", employees);
        }

        /// <summary>
        /// メンバー情報を更新します。
        /// </summary>
        [Command("upload", "メンバー情報を更新します。")]
        public async ValueTask UploadEmployeeAsync()
        {
            var employees = Enumerable.Range(1, 9)
                .Select(i => new EmployeeData($"100{i}", $"User {i}", $"User {i}", $"{i}000", $"100{i}@example.com", "男", new(1990, 1, 1), "A", new(2012, 4, 1)));
            int taskId = await _kaonaviService.UpdateMemberDataAsync(employees.Select(e => e.ToMemberData()).ToArray(), Context.CancellationToken).ConfigureAwait(false);
            Context.Logger.LogInformation("Start task at (TaskId: {taskId})", taskId);
        }

        /// <summary>
        /// タスクの進捗状況を取得します。
        /// </summary>
        /// <param name="taskId">タスクID</param>
        [Command("progress", "タスクの進捗状況を取得します。")]
        public async ValueTask FetchProgressAsync([Option("t", "タスクID")] int taskId)
        {
            var progress = await _kaonaviService.FetchTaskProgressAsync(taskId, Context.CancellationToken).ConfigureAwait(false);
            Context.Logger.LogInformation("Received Progress: {progress}", progress);
        }
    }
}
