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

        public async ValueTask RunAsync()
        {
            var memberLayout = await _kaonaviService.FetchMemberLayoutAsync(Context.CancellationToken).ConfigureAwait(false);
            Context.Logger.LogInformation(JsonSerializer.Serialize(memberLayout));
        }
    }
}
