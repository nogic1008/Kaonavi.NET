using System.CommandLine;
using System.CommandLine.Invocation;
using Kaonavi.Net;
using Microsoft.Extensions.Logging;

namespace ConsoleAppSample.Commands;

/// <summary>
/// メンバー情報のレイアウトを取得します。
/// </summary>
internal class LayoutCommand : Command
{
    /// <summary>
    /// LayoutCommandの新しいインスタンスを生成します。
    /// </summary>
    public LayoutCommand() : base("layout", "メンバー情報のレイアウトを取得します。") { }

    /// <summary>
    /// <see cref="IKaonaviClient"/>を使用してメンバー情報のレイアウトを取得します。
    /// </summary>
    /// <remarks><see cref="LayoutCommand"/>の実装部分です。DIにより<see cref="Command.Handler"/>に渡されます。</remarks>
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
            var memberLayout = await client.Layout.ReadMemberLayoutAsync(context.GetCancellationToken()).ConfigureAwait(false);
            logger.LogInformation("Received Layout: {memberLayout}", memberLayout);
            return 0;
        }
    }
}
