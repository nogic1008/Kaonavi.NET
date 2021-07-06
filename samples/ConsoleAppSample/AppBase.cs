using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Kaonavi.Net.Services;
using Microsoft.Extensions.Logging;

namespace ConsoleAppSample
{
    public class AppBase : ConsoleAppBase
    {
        private readonly IKaonaviService _kaonaviService;
        public AppBase(IKaonaviService kaonaviService) => _kaonaviService = kaonaviService;

        [Command("layout")]
        public async ValueTask FetchLayoutAsync()
        {
            var memberLayout = await _kaonaviService.FetchMemberLayoutAsync(Context.CancellationToken).ConfigureAwait(false);
            Context.Logger.LogInformation("Received Layout: {0}", JsonSerializer.Serialize(memberLayout));
        }

        [Command("download")]
        public async ValueTask DownloadEmployeeAsync()
        {
            var employees = (await _kaonaviService.FetchMembersDataAsync(Context.CancellationToken).ConfigureAwait(false))
                .Select(m => new EmployeeData(m));
            Context.Logger.LogInformation("Received Data: {employees}", employees);
        }

        [Command("upload")]
        public async ValueTask UploadEmployeeAsync()
        {
            var employees = Enumerable.Range(1, 9)
                .Select(i => new EmployeeData($"100{i}", $"User {i}", $"User {i}", $"{i}000", $"100{i}@example.com", "男", new(1990, 1, 1), "A", new(2012, 4, 1)));
            int taskId = await _kaonaviService.ReplaceMemberDataAsync(employees.Select(e => e.ToMemberData()).ToArray(), Context.CancellationToken).ConfigureAwait(false);
            Context.Logger.LogInformation("Start task at (TaskId: {taskId})", taskId);
        }

        [Command("progress")]
        public async ValueTask FetchProgressAsync(int taskId)
        {
            var progress = await _kaonaviService.FetchTaskProgressAsync(taskId, Context.CancellationToken).ConfigureAwait(false);
            Context.Logger.LogInformation("Received Progress: {progress}", progress);
        }
    }
}
