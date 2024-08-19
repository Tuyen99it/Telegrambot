using Advance.Abstract;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

public abstract class ReceiverServiceBase<TUpdateHandler> : IReceiverService where TUpdateHandler : IUpdateHandler
{
    // ReceiverService is implemented from BotClient and Update Handler ( dependency). We use contructor to make loose dependency between objects
    /// <summary>
    /// An abstract class to compose Receiver Service and Update Handler classes
    /// </summary>
    /// <typeparam name="TUpdateHandler">Update Handler to use in Update Receiver</typeparam>
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;
    private readonly ILogger _logger;
    internal ReceiverServiceBase(ITelegramBotClient botClient, TUpdateHandler updateHandler, ILogger<ReceiverServiceBase<TUpdateHandler>> logger)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
        _logger = logger;
    }

    /// <summary>
    /// Start to service Updates with proviced Update Handler classes
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>

    public async Task ReceiverAsync(CancellationToken stoppingToken)
    {
        //TODO: We can inject ReceiverOptions through IOption Container
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = [],// allow all update fields
            DropPendingUpdates = true,
        };
        var me = await _botClient.GetMeAsync(stoppingToken);
        _logger.LogInformation("Start receiving updates for {botname}", me.Username ?? "My awesome Bot");

        // start receiving updates
        await _botClient.ReceiveAsync(updateHandler: _updateHandler, receiverOptions: receiverOptions, cancellationToken: stoppingToken);

    }

}