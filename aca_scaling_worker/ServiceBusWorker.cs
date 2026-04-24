using aca_scaling_worker.Configuration;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace aca_scaling_worker
{
    public class ServiceBusWorker : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSettings _settings;
        private readonly IMessageHandler _messageHandler;
        private ServiceBusProcessor? _processor;
        private readonly ILogger<ServiceBusWorker> _logger;

        public ServiceBusWorker(
            ServiceBusClient client,
            IOptions<ServiceBusSettings> settings,
            IMessageHandler messageHandler,
            ILogger<ServiceBusWorker> logger)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(messageHandler);
            ArgumentNullException.ThrowIfNull(logger);
            _client = client;
            _settings = settings.Value;
            _messageHandler = messageHandler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor = _client.CreateProcessor(_settings.QueueName, new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 5,
                AutoCompleteMessages = false
            });

            _processor.ProcessMessageAsync += _messageHandler.HandleMessageAsync;
            _processor.ProcessErrorAsync += HandleErrorAsync;

            await _processor.StartProcessingAsync(stoppingToken);

            // Keep the service alive
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private Task HandleErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Error occurred while processing message");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            if (_processor != null)
            {
                await _processor.StopProcessingAsync(stoppingToken);
                await _processor.DisposeAsync();
            }

            await base.StopAsync(stoppingToken);
        }
    }
}
