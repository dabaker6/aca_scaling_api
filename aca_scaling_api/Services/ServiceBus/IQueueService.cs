using aca_scaling_api.Interfaces;
using aca_scaling_api.Utils;

namespace aca_scaling_api.Services.ServiceBus
{
    public interface IQueueService
    {
        Task SendMessageAsync(MessageBatch messages, CancellationToken cancellationToken = default);

        Task<QueueContent> GetQueueLength(CancellationToken cancellationToken = default);
    }
}
