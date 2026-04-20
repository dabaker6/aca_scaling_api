namespace aca_scaling_api.Infrastructure.ServiceBus
{
    public interface IQueueService
    {
        Task SendMessageAsync(string payload);
    }
}
