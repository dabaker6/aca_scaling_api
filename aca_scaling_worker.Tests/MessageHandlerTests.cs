using aca_scaling_worker;
using aca_scaling_worker.Configuration;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace aca_scaling_worker.Tests
{
    public class MessageHandlerTests
    {
        private readonly Mock<ILogger<MessageHandler>> _mockLogger;
        private readonly MessageHandler _messageHandler;
        private readonly IOptions<ServiceBusSettings> _settings;



        public MessageHandlerTests()
        {
            var settings = new ServiceBusSettings
            {
                FullyQualifiedNamespace = "test-namespace.servicebus.windows.net",
                QueueName = "test-queue",
                ProcessingTime = 500
            };

            _settings = Options.Create(settings);
            _mockLogger = new Mock<ILogger<MessageHandler>>();
            _messageHandler = new MessageHandler(_mockLogger.Object,_settings);
        }

        private static (ProcessMessageEventArgs args, Mock<ServiceBusReceiver> receiver) CreateArgs(string body = "test message")
        {
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString(body));
            var mockReceiver = new Mock<ServiceBusReceiver>();
            var args = new ProcessMessageEventArgs(message, mockReceiver.Object, CancellationToken.None);
            return (args, mockReceiver);
        }

        [Fact]
        public void Constructor_WithValidLogger_CreatesInstance()
        {
            _messageHandler.Should().NotBeNull();
        }

        [Fact]
        public async Task HandleMessageAsync_LogsInformation()
        {
            var (args, _) = CreateArgs();

            await _messageHandler.HandleMessageAsync(args);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleMessageAsync_CallsCompleteMessage()
        {
            var (args, mockReceiver) = CreateArgs();

            await _messageHandler.HandleMessageAsync(args);

            mockReceiver.Verify(
                x => x.CompleteMessageAsync(It.IsAny<ServiceBusReceivedMessage>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleMessageAsync_WhenCompleteMessageThrows_AbandonMessage()
        {
            var (args, mockReceiver) = CreateArgs();
            mockReceiver
                .Setup(x => x.CompleteMessageAsync(It.IsAny<ServiceBusReceivedMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            await _messageHandler.HandleMessageAsync(args);

            mockReceiver.Verify(
                x => x.AbandonMessageAsync(
                    It.IsAny<ServiceBusReceivedMessage>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleMessageAsync_WhenExceptionOccurs_LogsError()
        {
            var (args, mockReceiver) = CreateArgs();
            mockReceiver
                .Setup(x => x.CompleteMessageAsync(It.IsAny<ServiceBusReceivedMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            await _messageHandler.HandleMessageAsync(args);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
