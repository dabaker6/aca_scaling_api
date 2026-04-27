using aca_scaling_api.Interfaces;
using aca_scaling_api.Services;
using aca_scaling_api.Services.MessageGenerator;
using aca_scaling_api.Services.ServiceBus;
using aca_scaling_api.Validation;
using FluentAssertions;
using Moq;
using Xunit;

namespace aca_scaling_api.Tests.Endpoints
{
    public class SendMessagesEndpointTests
    {
        [Fact]
        public async Task MessageGenerator_GeneratesCorrectNumberOfMessages()
        {
            var messageGenerator = new MessageGeneratorService();
            var correlationId = "test-correlation";

            var messages = await messageGenerator.GenerateMessagesToQueueAsync(20, correlationId);

            messages.Should().HaveCount(20);
        }

        [Fact]
        public async Task QueueService_SendsMessageSuccessfully()
        {
            var mockQueueService = new Mock<IQueueService>();
            var messages = new List<MessageContent>
            {
                new MessageContent { WorkId = "1", JobId = "purchasetickets", CorrelationId = "test" }
            };


            await mockQueueService.Object.SendMessageAsync(messages);


            mockQueueService.Verify(x => x.SendMessageAsync(It.IsAny<IEnumerable<MessageContent>>()), Times.Once);
        }

        [Fact]
        public async Task RequestValidator_AcceptsValidMessageCount()
        {
            var validator = new SendMessageRequestValidator();
            var request = new SendMessageRequest(100);

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task RequestValidator_RejectsExcessiveMessageCount()
        {
            var validator = new SendMessageRequestValidator();
            var request = new SendMessageRequest(6000);

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
        }
    }
}
