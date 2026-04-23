using aca_scaling_api.Configuration;
using aca_scaling_api.Interfaces;
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

        public async Task<QueueContent> GetQueueLength()
        {
            
            QueueRuntimeProperties runtimeProperties = await _adminClient.GetQueueRuntimePropertiesAsync(_settings.QueueName);

            return new QueueContent(runtimeProperties);

        }

        public async Task SendMessageAsync(string payload)
        {
            var message = new ServiceBusMessage(payload);
            await _sender.SendMessageAsync(message);
        }


    }
}
