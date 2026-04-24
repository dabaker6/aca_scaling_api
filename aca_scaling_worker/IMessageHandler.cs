using Azure.Messaging.ServiceBus;

namespace aca_scaling_worker
{
    public interface IMessageHandler
    {
        Task HandleMessageAsync(ProcessMessageEventArgs args);
    }
}
