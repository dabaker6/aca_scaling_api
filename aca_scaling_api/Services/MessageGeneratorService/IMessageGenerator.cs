using aca_scaling_api.Interfaces;
using aca_scaling_api.Utils;

namespace aca_scaling_api.Services.MessageGenerator
{
    public interface IMessageGenerator
    {
        Task<MessageBatch> GenerateMessagesToQueueAsync(int messageCount, string correlationId);
    }
}
