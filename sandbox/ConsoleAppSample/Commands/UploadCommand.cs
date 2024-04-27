using System.CommandLine;
using System.CommandLine.Invocation;
using ConsoleAppSample.Entities;
using Kaonavi.Net;
using Microsoft.Extensions.Logging;

namespace ConsoleAppSample.Commands;

/// <summary>
/// メンバー情報を更新します。
/// </summary>
internal class UploadCommand : Command
{
    /// <summary>
    /// UploadCommandの新しいインスタンスを生成します。
    /// </summary>
    public UploadCommand() : base("upload", "メンバー情報を更新します。") { }

    /// <summary>
    /// <see cref="IKaonaviClient"/>を使用してメンバー情報を更新します。
    /// </summary>
    /// <remarks><see cref="UploadCommand"/>の実装部分です。DIにより<see cref="Command.Handler"/>に渡されます。</remarks>
    /// <param name="client"><see cref="IKaonaviClient"/>の実装</param>
    /// <param name="logger">ロガー</param>
    internal class CommandHandler(IKaonaviClient client, ILogger logger) : ICommandHandler
    {
        /// <inheritdoc />
        public int Invoke(InvocationContext context) => InvokeAsync(context).GetAwaiter().GetResult();

        /// <inheritdoc />
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            context.GetCancellationToken().ThrowIfCancellationRequested();
            var employees = Enumerable.Range(1, 9)
                .Select(i => new EmployeeData($"100{i}", $"User {i}", $"User {i}", $"{i}000", $"100{i}@example.com", "男", new(1990, 1, 1), "A", new(2012, 4, 1)));
            var customSheets = Enumerable.Range(1, 9)
                .Select(i => new CustomSheetData($"100{i}", $"100-000{i}", $"Address {i}", new(2023, 1, i)));
            int taskId1 = await client.Member.UpdateAsync(employees.Select(e => e.ToMemberData()).ToArray(), context.GetCancellationToken()).ConfigureAwait(false);
            logger.LogInformation("Start task at (TaskId: {taskId})", taskId1);
            int taskId2 = await client.Sheet.UpdateAsync(1, customSheets.ToSingleSheetData(), context.GetCancellationToken()).ConfigureAwait(false);
            logger.LogInformation("Start task at (TaskId: {taskId})", taskId2);
            return 0;
        }
    }
}
