using aca_scaling_api.Configuration;
using aca_scaling_api.Interfaces;
using aca_scaling_api.Utils;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Options;

namespace aca_scaling_api.Services.ServiceBus
{
    public class QueueService : IQueueService
    {
        private readonly ServiceBusSettings _settings;
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusAdministrationClient _adminClient;

        public QueueService(IOptions<ServiceBusSettings> settings, ServiceBusClient client, ServiceBusAdministrationClient adminClient)
        {
            _settings = settings.Value;
            _sender = client.CreateSender(_settings.QueueName);
            _adminClient = adminClient;
        }

        public async Task<QueueContent> GetQueueLength(CancellationToken cancellationToken = default)
        {
            
            QueueRuntimeProperties runtimeProperties = await _adminClient.GetQueueRuntimePropertiesAsync(_settings.QueueName);

            return new QueueContent(runtimeProperties);

        }

        public async Task SendMessageAsync(MessageBatch generatedMessages, CancellationToken cancellationToken = default)
        {
            if (generatedMessages.Messages == null || !generatedMessages.Messages.Any())
            {
                throw new ArgumentException("Generated messages cannot be null or empty.");
            }

            foreach (var messageGroup in generatedMessages.Messages)
            {
                
                if (messageGroup == null || !messageGroup.Any())
                {
                    throw new ArgumentException("Message groups cannot be null or empty.");
                }

                Queue<ServiceBusMessage> messages = new();

                foreach (var generatedMessage in messageGroup)
                {
                    messages.Enqueue(new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(generatedMessage)));
                }

                //total number of messages to be sent to the Service Bus queue, used for logging and exception messages
                int messageCount = messages.Count;

                // while all messages are not sent to the Service Bus queue
                while (messages.Count > 0)
                {

                    // start a new batch
                    using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();
                    Guid batchId = Guid.NewGuid();

                    ServiceBusMessage message = messages.Peek();
                    message.ApplicationProperties.Add("BatchId", batchId);

                    // add the first message to the batch
                    if (messageBatch.TryAddMessage(message))
                    {
                        // dequeue the message from the .NET queue once the message is added to the batch
                        messages.Dequeue();
                    }
                    else
                    {
                        // if the first message can't fit, then it is too large for the batch
                        throw new Exception($"Message {messageCount - messages.Count} is too large and cannot be sent.");
                    }

                    // add as many messages as possible to the current batch
                    while (messages.Count > 0 && messageBatch.TryAddMessage(messages.Peek()))
                    {
                        // dequeue the message from the .NET queue as it has been added to the batch
                        messages.Dequeue();
                    }

                    // now, send the batch
                    await _sender.SendMessagesAsync(messageBatch);

                    // if there are any remaining messages in the .NET queue, the while loop repeats
                }
                // add delay to replicate real world scenario where messages are generated at different times, and to avoid overwhelming the Service Bus queue with messages
                await Task.Delay(_settings.ProcessingTime,cancellationToken);
            }
        }
    }
}
