using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using ConsoleAppSample;
using ConsoleAppSample.Commands;
using Kaonavi.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var root = new RootCommand()
{
    new LayoutCommand(),
    new DownloadCommand(),
    new UploadCommand(),
    new ProgressCommand(),
};
var parser = new CommandLineBuilder(root)
    .UseDefaults()
    .UseHost(host =>
    {
        host
            .ConfigureDefaults(args)
            .ConfigureServices((ctx, services) =>
            {
                // IOptions
                var config = ctx.Configuration;
                services.Configure<KaonaviOptions>(config.GetSection(nameof(KaonaviOptions)));

                // DI
                services.AddLogging(services => services.AddConsole());
                services.AddHttpClient<IKaonaviClient, KaonaviClient>((client, provider) =>
                {
                    var options = provider.GetRequiredService<IOptions<KaonaviOptions>>().Value;
                    return new(client, options.ConsumerKey, options.ConsumerSecret)
                    {
                        UseDryRun = options.UseDryRun
                    };
                });
            })
            .UseCommandHandler<LayoutCommand, LayoutCommand.CommandHandler>()
            .UseCommandHandler<DownloadCommand, DownloadCommand.CommandHandler>()
            .UseCommandHandler<UploadCommand, UploadCommand.CommandHandler>()
            .UseCommandHandler<ProgressCommand, ProgressCommand.CommandHandler>();
        return;
    })
    .Build();
return await parser.InvokeAsync(args);
