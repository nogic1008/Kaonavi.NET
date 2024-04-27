using System.CommandLine;
using System.CommandLine.Invocation;
using Kaonavi.Net;
using Microsoft.Extensions.Logging;

namespace ConsoleAppSample.Commands;

/// <summary>
/// タスクの進捗状況を取得します。
/// </summary>
internal class ProgressCommand : Command
{
    /// <summary>
    /// ProgressCommandの新しいインスタンスを生成します。
    /// </summary>
    public ProgressCommand() : base("progress", "タスクの進捗状況を取得します。")
        => AddOption(new Option<int>("--task-id", "タスクID") { IsRequired = true });

    /// <summary>
    /// <see cref="IKaonaviClient"/>を使用してタスクの進捗状況を取得します。
    /// </summary>
    /// <remarks><see cref="ProgressCommand"/>の実装部分です。DIにより<see cref="Command.Handler"/>に渡されます。</remarks>
    /// <param name="client"><see cref="IKaonaviClient"/>の実装</param>
    /// <param name="logger">ロガー</param>
    internal class CommandHandler(IKaonaviClient client, ILogger logger) : ICommandHandler
    {
        /// <summary>タスクID (DI対象)</summary>
        public int TaskId { get; init; }

        /// <inheritdoc />
        public int Invoke(InvocationContext context) => InvokeAsync(context).GetAwaiter().GetResult();

        /// <inheritdoc />
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            context.GetCancellationToken().ThrowIfCancellationRequested();
            var progress = await client.Task.ReadAsync(TaskId, context.GetCancellationToken()).ConfigureAwait(false);
            logger.LogInformation("Received Progress: {progress}", progress);
            return 0;
        }
    }
}
