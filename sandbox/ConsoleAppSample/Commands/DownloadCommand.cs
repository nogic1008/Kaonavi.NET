using System.CommandLine;
using System.CommandLine.Invocation;
using ConsoleAppSample.Entities;
using Kaonavi.Net;
using Microsoft.Extensions.Logging;

namespace ConsoleAppSample.Commands;

/// <summary>
/// メンバー情報を全取得します。
/// </summary>
internal class DownloadCommand : Command
{
    /// <summary>
    /// DownloadCommandの新しいインスタンスを生成します。
    /// </summary>
    public DownloadCommand() : base("download", "メンバー情報を全取得します。") { }

    /// <summary>
    /// <see cref="IKaonaviClient"/>を使用してメンバー情報を全取得します。
    /// </summary>
    /// <remarks><see cref="DownloadCommand"/>の実装部分です。DIにより<see cref="Command.Handler"/>に渡されます。</remarks>
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
            var employees = (await client.Member.ListAsync(context.GetCancellationToken()).ConfigureAwait(false))
                .Select(m => new EmployeeData(m));
            logger.LogInformation("Received Data: {employees}", employees);
            return 0;
        }
    }
}
