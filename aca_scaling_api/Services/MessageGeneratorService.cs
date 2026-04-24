using aca_scaling_api.Interfaces;

namespace aca_scaling_api.Services
{
    public class MessageGeneratorService : IMessageGenerator
    {
        public Task<IEnumerable<MessageContent>> GenerateMessagesToQueueAsync(int messageCount, string correlationId)
        {
            IEnumerable<MessageContent> messages = Enumerable.Empty<MessageContent>();
            
            for (int i = 0; i < messageCount; i++)
            {
                messages = messages.Concat(new[] 
                { 
                    new MessageContent 
                    { 
                        WorkId = Guid.NewGuid().ToString(), 
                        JobId = "purchasetickets",
                        CorrelationId = correlationId
                    } 
                }
                );
            }

            return Task.FromResult(messages);
        }
    }
}
