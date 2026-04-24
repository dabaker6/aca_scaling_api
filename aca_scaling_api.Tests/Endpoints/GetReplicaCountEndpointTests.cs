using aca_scaling_api.Tests.Helpers;
using Azure;
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace aca_scaling_api.Tests.Endpoints
{
    public class GetReplicaCountEndpointTests : IClassFixture<TestApiFactory>
    {
        private readonly TestApiFactory _factory;
        private readonly HttpClient _client;

        public GetReplicaCountEndpointTests(TestApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetReplicaCount_WithValidRevisionName_Returns200()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetReplicaCount("my-revision-1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            var response = await _client.GetAsync("/api/v1/replicas/my-revision-1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetReplicaCount_WithValidRevisionName_ReturnsCount()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetReplicaCount("my-revision-1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            var response = await _client.GetAsync("/api/v1/replicas/my-revision-1");
            var count = await response.Content.ReadFromJsonAsync<int>();

            count.Should().Be(5);
        }

        [Fact]
        public async Task GetReplicaCount_WithInvalidCharactersInName_Returns400()
        {
            var response = await _client.GetAsync("/api/v1/replicas/invalid_name!");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetReplicaCount_WhenServiceThrowsRequestFailedException_ReturnsAzureStatusCode()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetReplicaCount(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(404, "Not Found"));

            var response = await _client.GetAsync("/api/v1/replicas/my-revision-1");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReplicaCount_WhenServiceThrowsUnexpectedException_Returns500()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetReplicaCount(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Unexpected error"));

            var response = await _client.GetAsync("/api/v1/replicas/my-revision-1");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
