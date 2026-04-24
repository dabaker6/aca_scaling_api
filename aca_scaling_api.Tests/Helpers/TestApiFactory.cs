using aca_scaling_api.Interfaces;
using aca_scaling_api.Services;
using aca_scaling_api.Services.ContainerApps;
using aca_scaling_api.Services.ServiceBus;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Azure.ResourceManager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace aca_scaling_api.Tests.Helpers
{
    public class TestApiFactory : WebApplicationFactory<Program>
    {
        public Mock<IQueueService> MockQueueService { get; } = new();
        public Mock<IContainerAppsService> MockContainerAppsService { get; } = new();
        public Mock<IMessageGenerator> MockMessageGenerator { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ServiceBus:FullyQualifiedNamespace"] = "test.servicebus.windows.net",
                    ["ServiceBus:QueueName"] = "test-queue",
                    ["ContainerApps:SubscriptionId"] = "test-sub-id",
                    ["ContainerApps:ResourceGroup"] = "test-resource-group",
                    ["ContainerApps:ContainerAppName"] = "test-app",
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IQueueService>();
                services.RemoveAll<IContainerAppsService>();
                services.RemoveAll<IMessageGenerator>();
                services.RemoveAll<ServiceBusClient>();
                services.RemoveAll<ServiceBusAdministrationClient>();
                services.RemoveAll<ArmClient>();

                services.AddSingleton(MockQueueService.Object);
                services.AddSingleton(MockContainerAppsService.Object);
                services.AddSingleton(MockMessageGenerator.Object);
            });
        }
    }
}
