using aca_scaling_api.Services.BackgroundTaskQueue;

namespace aca_scaling_api.Services.HostedBackgroundService
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> _logger;        
        public IBackgroundTaskQueue TaskQueue { get; }

        public QueuedHostedService(ILogger<QueuedHostedService> logger, IBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            TaskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) 
            { 
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");
            await base.StopAsync(cancellationToken);
        }
    }
}
