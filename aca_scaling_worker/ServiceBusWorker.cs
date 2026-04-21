using aca_scaling_api.Configuration;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace aca_scaling_worker
{
    public class ServiceBusWorker : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSettings _settings;
        private ServiceBusProcessor _processor;
        private ILogger<ServiceBusWorker> _logger;

        public ServiceBusWorker(ServiceBusClient client, IOptions<ServiceBusSettings> settings, ILogger<ServiceBusWorker> logger)
        {
            _client = client;
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor = _client.CreateProcessor(_settings.QueueName, new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 5,
                AutoCompleteMessages = false
            });

            _processor.ProcessMessageAsync += HandleMessageAsync;
            _processor.ProcessErrorAsync += HandleErrorAsync;

            await _processor.StartProcessingAsync(stoppingToken);

            // Keep the service alive
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleMessageAsync(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();

            try
            {
                // your logic here
                _logger.LogInformation("Processing message: {MessageBody}", body);
                await Task.Delay(1000); // Simulate work
                await args.CompleteMessageAsync(args.Message);
            }
            catch
            {
                await args.AbandonMessageAsync(args.Message);
            }
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
