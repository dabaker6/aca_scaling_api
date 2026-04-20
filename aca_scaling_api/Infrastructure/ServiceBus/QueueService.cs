using aca_scaling_api.Configuration;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace aca_scaling_api.Infrastructure.ServiceBus
{
    public class QueueService : IQueueService
    {
        private readonly ServiceBusSettings _settings;
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusProcessor _processor;

        public QueueService(IOptions<ServiceBusSettings> settings, ServiceBusClient client)
        {
            _settings = settings.Value;
            _sender = client.CreateSender(_settings.QueueName);
            _processor = client.CreateProcessor(_settings.QueueName);
        }

        public async Task SendMessageAsync(string payload)
        {
            var message = new ServiceBusMessage(payload);
            await _sender.SendMessageAsync(message);
        }

        public async Task ReceiveMessageAsync()
        {
            var message = await _processor.ProcessMessageAsync();
        }
    }
}
