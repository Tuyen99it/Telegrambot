using Microsoft.Extensions.Options;
using Telegram.Bot;
using Console.Advance;

// creat an instance of CreateApplicationBuilder
var builder = Host.CreateDefaultBuilder(args);
//register bot configuration
IHost host = builder.ConfigureServices((context, services) =>
{
    // register Bot Configure
    services.Configure<BotConfiguration>(context.Configuration.GetSection("BotConfiguration"));

    //register named httpClient to benefits from IHttpClientFactory and consume it with ITelegramBotClient typed client
    // More read:
    //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
    //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests

    services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                BotConfiguration? botConfiguration = sp.GetService<IOptions<BotConfiguration>>()?.Value;
                ArgumentNullException.ThrowIfNull(botConfiguration);
                TelegramBotClientOptions options = new(botConfiguration.BotToken);
                return new TelegramBotClient(options, httpClient);
            });

}).Build();
await host.RunAsync();

// var builder = Host.CreateApplicationBuilder(args);
// builder.Services.AddHostedService<Worker>();

// var host = builder.Build();
// host.Run();
