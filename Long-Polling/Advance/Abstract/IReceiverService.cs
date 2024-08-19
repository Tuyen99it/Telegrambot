namespace Advance.Abstract;
///<summary>
///A marker interface for Update Receiver service
///</summary>
public interface IReceiverService
{
    Task ReceiverAsync(CancellationToken stoppingToken);
}