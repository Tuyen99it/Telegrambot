
using System.ComponentModel;

namespace Advance.Abstract;

// A background service consuming a scoped service
// see more: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services#consuming-a-scoped-service-in-a-background-task
public abstract class PollingServiceBase<TReceiverService> : BackgroundService where TReceiverService : IReceiverService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    internal PollingServiceBase(IServiceProvider serviceProvider, ILogger<PollingServiceBase<TReceiverService>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Polling service");
        await DoWork(stoppingToken);
    }
    private async Task DoWork(CancellationToken stoppingToken)
    {
        // make sure we receive update until Cancellation Requested,
        // no matter what errors our receiveAsync get
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // create a new IServiceScope on each interaction
                // This way we can leverage benefits of scoped TReciverService and typed HttpClient - we'll grab "fresh" instance each time
                using var scope = _serviceProvider.CreateScope();
                // get implementation type from serviceType
                var receiver = scope.ServiceProvider.GetRequiredService<TReceiverService>();// get service type, create exception if TReceiverService is null
                // get update from receiverService Base
                await receiver.ReceiveAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Polling false with exception {Exception}", ex);
                // Cooldown if something goes wrong
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}