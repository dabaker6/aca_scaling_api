using aca_scaling_api.Tests.Helpers;
using Azure;
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace aca_scaling_api.Tests.Endpoints
{
    public class GetRevisionNameEndpointTests : IClassFixture<TestApiFactory>
    {
        private readonly TestApiFactory _factory;
        private readonly HttpClient _client;

        public GetRevisionNameEndpointTests(TestApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetRevisionName_WhenRevisionExists_Returns200()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetRevisionName(It.IsAny<CancellationToken>()))
                .ReturnsAsync("my-app--revision-abc123");

            var response = await _client.GetAsync("/api/v1/revisionName/");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRevisionName_WhenRevisionExists_ReturnsName()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetRevisionName(It.IsAny<CancellationToken>()))
                .ReturnsAsync("my-app--revision-abc123");

            var response = await _client.GetAsync("/api/v1/revisionName/");
            var name = await response.Content.ReadFromJsonAsync<string>();

            name.Should().Be("my-app--revision-abc123");
        }

        [Fact]
        public async Task GetRevisionName_WhenRevisionIsNull_Returns404()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetRevisionName(It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(string));

            var response = await _client.GetAsync("/api/v1/revisionName/");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetRevisionName_WhenServiceThrowsRequestFailedException_ReturnsAzureStatusCode()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetRevisionName(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(401, "Unauthorized"));

            var response = await _client.GetAsync("/api/v1/revisionName/");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetRevisionName_WhenServiceThrowsUnexpectedException_Returns500()
        {
            _factory.MockContainerAppsService
                .Setup(x => x.GetRevisionName(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var response = await _client.GetAsync("/api/v1/revisionName/");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
