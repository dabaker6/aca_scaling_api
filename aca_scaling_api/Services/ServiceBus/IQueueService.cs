using aca_scaling_api.Interfaces;

namespace aca_scaling_api.Services.ServiceBus
{
    public interface IQueueService
    {
        Task SendMessageAsync(IEnumerable<MessageContent> messages, CancellationToken cancellationToken = default);

        Task<QueueContent> GetQueueLength(CancellationToken cancellationToken = default);
    }
}
