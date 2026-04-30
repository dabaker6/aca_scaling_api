using aca_scaling_api.Interfaces;

namespace aca_scaling_api.Utils
{
    public class MessageBatch
    {
        public IEnumerable<IEnumerable<MessageContent>> Messages { get; set; }
        public int TotalMessageCount { get; set; }

        public MessageBatch(IEnumerable<IEnumerable<MessageContent>> messages, int totalMessageCount)
        {
            Messages = messages;
            TotalMessageCount = totalMessageCount;
        }
    }
}
