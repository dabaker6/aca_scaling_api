using aca_scaling_api.Interfaces;

namespace aca_scaling_api.Utils
{
    public static class MessageGenerator
    {
        public static async Task<IEnumerable<MessageContent>> GenerateMessagesToQueue()
        {
            IEnumerable<MessageContent> messages = Enumerable.Empty<MessageContent>();
            
            for (int i = 0; i < 20; i++)
            {
                messages = messages.Concat(new[] { new MessageContent { 
                    workId = Guid.NewGuid().ToString(), 
                    jobId = "purchasetickets" } 
                }
                );
            }

            return messages;
        }
    }
}
