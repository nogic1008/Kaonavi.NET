using ConsoleAppFramework;
using ConsoleAppSample;
using Kaonavi.Net.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

await Host.CreateDefaultBuilder()
    .ConfigureLogging(logging => logging.ReplaceToSimpleConsole())
    .ConfigureServices((context, services) => {
        // IOptions
        var config = context.Configuration;
        services.Configure<KaonaviOptions>(config.GetSection(nameof(KaonaviOptions)));

        // DI
        services.AddHttpClient<IKaonaviService, KaonaviV2Service>((client, provider) => {
            var options = provider.GetRequiredService<IOptions<KaonaviOptions>>().Value;
            return new(client, options.ConsumerKey, options.ConsumerSecret)
            {
                UseDryRun = options.UseDryRun
            };
        });
    })
    .RunConsoleAppFrameworkAsync<AppBase>(args)
    .ConfigureAwait(false);
