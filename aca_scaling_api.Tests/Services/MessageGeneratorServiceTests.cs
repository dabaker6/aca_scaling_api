using aca_scaling_api.Services;
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
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(20, "test-correlation-id");

            result.Should().NotBeNull();
            result.Should().HaveCount(20);
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_WithDifferentCount_ReturnsCorrectCount()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(5, "test-correlation-id");

            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_WithZeroCount_ReturnsEmpty()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(0, "test-correlation-id");

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_AllMessagesHaveCorrectCorrelationId()
        {
            var correlationId = "test-correlation-id";

            var result = await _messageGenerator.GenerateMessagesToQueueAsync(10, correlationId);

            result.Should().AllSatisfy(msg => msg.CorrelationId.Should().Be(correlationId));
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_AllMessagesHaveJobId()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(10, "test-correlation-id");

            result.Should().AllSatisfy(msg => msg.JobId.Should().Be("purchasetickets"));
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_AllMessagesHaveUniqueWorkIds()
        {
            var result = await _messageGenerator.GenerateMessagesToQueueAsync(20, "test-correlation-id");

            var workIds = result.Select(msg => msg.WorkId).ToList();
            workIds.Distinct().Should().HaveCount(20);
        }

        [Fact]
        public async Task GenerateMessagesToQueueAsync_WithDifferentCorrelationIds_ProducesMessages()
        {
            var correlationId1 = "correlation-1";
            var correlationId2 = "correlation-2";

            var result1 = await _messageGenerator.GenerateMessagesToQueueAsync(5, correlationId1);
            var result2 = await _messageGenerator.GenerateMessagesToQueueAsync(5, correlationId2);

            result1.Should().AllSatisfy(msg => msg.CorrelationId.Should().Be(correlationId1));
            result2.Should().AllSatisfy(msg => msg.CorrelationId.Should().Be(correlationId2));
        }
    }
}
