using Azure.Messaging.ServiceBus.Administration;

namespace aca_scaling_api.Interfaces
{
    public class QueueContent
    {
        public QueueContent(QueueRuntimeProperties runtimeProperties)
        {
            ActiveMessageCount = runtimeProperties.ActiveMessageCount.ToString();
        }

        public string ActiveMessageCount { get; set; } = string.Empty;
    }
}
