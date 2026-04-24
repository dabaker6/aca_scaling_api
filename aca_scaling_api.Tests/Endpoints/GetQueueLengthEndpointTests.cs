using aca_scaling_api.Interfaces;
using aca_scaling_api.Tests.Helpers;
using Azure;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace aca_scaling_api.Tests.Endpoints
{
    public class GetQueueLengthEndpointTests : IClassFixture<TestApiFactory>
    {
        private readonly TestApiFactory _factory;
        private readonly HttpClient _client;

        public GetQueueLengthEndpointTests(TestApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static QueueContent MakeQueueContent(long activeCount)
        {
            var props = ServiceBusModelFactory.QueueRuntimeProperties("test-queue", activeMessageCount: activeCount);
            return new QueueContent(props);
        }

        [Fact]
        public async Task GetQueueLength_WhenServiceReturnsData_Returns200()
        {
            _factory.MockQueueService
                .Setup(x => x.GetQueueLength())
                .ReturnsAsync(MakeQueueContent(10));

            var response = await _client.GetAsync("/api/v1/queue-length/");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetQueueLength_WhenServiceReturnsData_ReturnsActiveMessageCount()
        {
            _factory.MockQueueService
                .Setup(x => x.GetQueueLength())
                .ReturnsAsync(MakeQueueContent(42));

            var response = await _client.GetAsync("/api/v1/queue-length/");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            json.GetProperty("activeMessageCount").GetString().Should().Be("42");
        }

        [Fact]
        public async Task GetQueueLength_WhenServiceThrowsRequestFailedException_ReturnsAzureStatusCode()
        {
            _factory.MockQueueService
                .Setup(x => x.GetQueueLength())
                .ThrowsAsync(new RequestFailedException(503, "Service Unavailable"));

            var response = await _client.GetAsync("/api/v1/queue-length/");

            response.StatusCode.Should().Be((HttpStatusCode)503);
        }

        [Fact]
        public async Task GetQueueLength_WhenServiceThrowsUnexpectedException_Returns500()
        {
            _factory.MockQueueService
                .Setup(x => x.GetQueueLength())
                .ThrowsAsync(new InvalidOperationException("Unexpected error"));

            var response = await _client.GetAsync("/api/v1/queue-length/");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
