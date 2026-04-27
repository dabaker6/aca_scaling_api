using aca_scaling_api.Configuration;
using aca_scaling_api.Interfaces;
using aca_scaling_api.Services.ServiceBus;
using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace aca_scaling_api.Tests.Services.ServiceBus
{
    public class QueueServiceTests
    {
        private readonly Mock<ServiceBusClient> _mockServiceBusClient;
        private readonly Mock<ServiceBusAdministrationClient> _mockAdminClient;
        private readonly Mock<ServiceBusSender> _mockSender;
        private readonly IOptions<ServiceBusSettings> _queueSettings;
        private readonly QueueService _queueService;

        public QueueServiceTests()
        {
            _mockServiceBusClient = new Mock<ServiceBusClient>();
            _mockAdminClient = new Mock<ServiceBusAdministrationClient>();
            _mockSender = new Mock<ServiceBusSender>();

            var settings = new ServiceBusSettings
            {
                FullyQualifiedNamespace = "test-namespace.servicebus.windows.net",
                QueueName = "test-queue"
            };

            _queueSettings = Options.Create(settings);

            _mockServiceBusClient
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_mockSender.Object);
            
            _queueService = new QueueService(_queueSettings, _mockServiceBusClient.Object, _mockAdminClient.Object);
        } 

        [Fact]
        public async Task GetQueueLength_ReturnsQueueContentWithActiveMessageCount()
        {
            var runtimeProps = ServiceBusModelFactory.QueueRuntimeProperties(
                name: "test-queue",
                activeMessageCount: 42);

            var mockResponse = new Mock<Response<QueueRuntimeProperties>>();
            mockResponse.Setup(r => r.Value).Returns(runtimeProps);

            _mockAdminClient
                .Setup(x => x.GetQueueRuntimePropertiesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);

            var result = await _queueService.GetQueueLength();

            result.Should().NotBeNull();
            result.ActiveMessageCount.Should().Be("42");
        }

        [Fact]
        public async Task GetQueueLength_WithEmptyQueue_ReturnsZeroCount()
        {
            var runtimeProps = ServiceBusModelFactory.QueueRuntimeProperties(
                name: "test-queue",
                activeMessageCount: 0);

            var mockResponse = new Mock<Response<QueueRuntimeProperties>>();
            mockResponse.Setup(r => r.Value).Returns(runtimeProps);

            _mockAdminClient
                .Setup(x => x.GetQueueRuntimePropertiesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);

            var result = await _queueService.GetQueueLength();

            result.ActiveMessageCount.Should().Be("0");
        }
    }
}
