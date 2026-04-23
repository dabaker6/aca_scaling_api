using aca_scaling_api.Interfaces;

namespace aca_scaling_api.Services.ServiceBus
{
    public interface IQueueService
    {
        Task SendMessageAsync(string payload);

        Task<QueueContent> GetQueueLength();
    }
}
