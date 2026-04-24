using Azure.Messaging.ServiceBus;

namespace aca_scaling_worker
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(ILogger<MessageHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleMessageAsync(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();

            try
            {
                _logger.LogInformation("Processing message: {MessageBody}", body);
                await Task.Delay(1000); // Simulate work
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
