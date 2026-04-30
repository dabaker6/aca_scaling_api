using aca_scaling_worker.Configuration;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace aca_scaling_worker
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;
        private readonly ServiceBusSettings _settings;

        public MessageHandler(ILogger<MessageHandler> logger, IOptions<ServiceBusSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task HandleMessageAsync(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();

            try
            {
                _logger.LogInformation("Processing message: {MessageBody}", body);
                                
                await Task.Delay(_settings.ProcessingTime); // Simulate work
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await args.AbandonMessageAsync(args.Message);
            }
        }
    }
}
