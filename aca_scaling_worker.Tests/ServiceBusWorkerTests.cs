using aca_scaling_worker.Configuration;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace aca_scaling_worker.Tests
{
    public class ServiceBusWorkerTests
    {
        private readonly Mock<ServiceBusClient> _mockServiceBusClient;
        private readonly Mock<IMessageHandler> _mockMessageHandler;
        private readonly Mock<ILogger<ServiceBusWorker>> _mockLogger;
        private readonly IOptions<ServiceBusSettings> _settings;
        private readonly ServiceBusWorker _worker;

        public ServiceBusWorkerTests()
        {
            _mockServiceBusClient = new Mock<ServiceBusClient>();
            _mockMessageHandler = new Mock<IMessageHandler>();
            _mockLogger = new Mock<ILogger<ServiceBusWorker>>();

            var settings = new ServiceBusSettings
            {
                FullyQualifiedNamespace = "test-namespace.servicebus.windows.net",
                QueueName = "test-queue"
            };

            _settings = Options.Create(settings);
            _worker = new ServiceBusWorker(_mockServiceBusClient.Object, _settings, _mockMessageHandler.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesService()
        {
            // Act & Assert
            _worker.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullServiceBusClient_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ServiceBusWorker(null!, _settings, _mockMessageHandler.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullSettings_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ServiceBusWorker(_mockServiceBusClient.Object, null!, _mockMessageHandler.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullMessageHandler_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ServiceBusWorker(_mockServiceBusClient.Object, _settings, null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ServiceBusWorker(_mockServiceBusClient.Object, _settings, _mockMessageHandler.Object, null!));
        }
    }
}
