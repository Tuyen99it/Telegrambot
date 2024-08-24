using Advance.Abstract;
using Telegram.Bot;
namespace Advance.Services;
public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger) : PollingServiceBase<ReceiverService>(serviceProvider, logger);