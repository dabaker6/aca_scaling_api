using aca_scaling_api.Interfaces;
using aca_scaling_api.Utils;

namespace aca_scaling_api.Services.MessageGenerator
{
    public class MessageGeneratorService : IMessageGenerator
    {
        public Task<MessageBatch> GenerateMessagesToQueueAsync(int messageCount, string correlationId)
        {
            IEnumerable<IEnumerable<MessageContent>> chunks = [];
            
            int[] generatedRandomArray = GenerateRandomArray(messageCount);
            int totalMessageCount = 0;

            foreach (int value in generatedRandomArray) 
            {
                IEnumerable<MessageContent> messages = [];
                for (int i = 0; i < value; i++)
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

                totalMessageCount += value;
                chunks = chunks.Append(messages);
                
            }

            MessageBatch messageBatch = new MessageBatch(chunks, totalMessageCount);                

            return Task.FromResult(messageBatch);
        }

        private int[] GenerateRandomArray(int length)
        {

            int number = length;
            int[] array = [];
            Random rand = new Random();

            while (number > 0)
            {
                int value = rand.Next(1, (int)(length * 0.01));
                array = array.Append(value).ToArray();
                number = number - value;
            }
            return array;
        }
    }
}
