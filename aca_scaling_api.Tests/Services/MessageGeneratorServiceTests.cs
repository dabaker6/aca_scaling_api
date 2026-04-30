using aca_scaling_api.Services.MessageGenerator;
using FluentAssertions;
using Xunit;

namespace aca_scaling_api.Tests.Services
{
    public class MessageGeneratorServiceTests
    {
        private readonly MessageGeneratorService _messageGenerator;

        public MessageGeneratorServiceTests()
        {
            _messageGenerator = new MessageGeneratorService();
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_ReturnsRequestedCount()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(1000, "test-correlation-id");

            result.Should().NotBeNull();
            result.TotalMessageCount.Should().BeCloseTo(1000, 20);
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_WithDifferentCount_ReturnsCorrectCount()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(500, "test-correlation-id");

            result.TotalMessageCount.Should().BeCloseTo(500, 20);
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_WithZeroCount_ReturnsEmpty()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(0, "test-correlation-id");

            result.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_AllMessagesHaveCorrectCorrelationId()
        {
            var correlationId = "test-correlation-id";

            var result = await _messageGenerator.GenerateMessagesToQueueAsync(1000, correlationId);

            result.Messages.Should().AllSatisfy(msg => msg.FirstOrDefault()?.CorrelationId.Should().Be(correlationId));
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_AllMessagesHaveJobId()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(1000, "test-correlation-id");

            result.Messages.Should().AllSatisfy(msg => msg.FirstOrDefault()?.JobId.Should().Be("purchasetickets"));
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_AllMessagesHaveUniqueWorkIds()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(2000, "test-correlation-id");

            var workIds = result.Messages.Select(msg => msg.FirstOrDefault()?.WorkId).ToList();
            workIds.Distinct().Should().HaveCount(workIds.Count());
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_WithDifferentCorrelationIds_ProducesMessages()
        {
            var correlationId1 = "correlation-1";
            var correlationId2 = "correlation-2";

            var result1 = await _messageGenerator.GenerateMessagesToQueueAsync(1000, correlationId1);
            var result2 = await _messageGenerator.GenerateMessagesToQueueAsync(1000, correlationId2);

            result1.Messages.Should().AllSatisfy(msg => msg.FirstOrDefault()?.CorrelationId.Should().Be(correlationId1));
            result2.Messages.Should().AllSatisfy(msg => msg.FirstOrDefault()?.CorrelationId.Should().Be(correlationId2));
        }
    }
}
