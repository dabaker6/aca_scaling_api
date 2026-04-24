using aca_scaling_api.Interfaces;

namespace aca_scaling_api.Services
{
    public interface IMessageGenerator
    {
        Task<IEnumerable<MessageContent>> GenerateMessagesToQueueAsync(int messageCount, string correlationId);
    }
}
