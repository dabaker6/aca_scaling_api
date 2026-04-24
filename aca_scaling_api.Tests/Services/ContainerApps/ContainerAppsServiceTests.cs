using aca_scaling_api.Configuration;
using aca_scaling_api.Services.ContainerApps;
using Azure.ResourceManager;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace aca_scaling_api.Tests.Services.ContainerApps
{
    public class ContainerAppsServiceTests
    {
        private readonly Mock<ArmClient> _mockArmClient;
        private readonly IOptions<ContainerAppsSettings> _containerAppsSettings;
        private readonly ContainerAppsService _service;

        public ContainerAppsServiceTests()
        {
            _mockArmClient = new Mock<ArmClient>();

            var settings = new ContainerAppsSettings
            {
                SubscriptionId = "test-subscription-id",
                ResourceGroup = "test-resource-group",
                ContainerAppName = "test-app"
            };

            _containerAppsSettings = Options.Create(settings);
            _service = new ContainerAppsService(_containerAppsSettings, _mockArmClient.Object);
        }

        [Fact]
        public void Constructor_WithValidSettings_InitializesService()
        {
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task GetReplicaCount_WithoutActiveRevision_ReturnsZero()
        {
            // _activeRevision is null until GetRevisionName is called, so returns 0
            var result = await _service.GetReplicaCount("test-revision", CancellationToken.None);

            result.Should().Be(0);
        }

        [Fact]
        public async Task GetScaleRules_ThrowsNotImplementedException()
        {
            Func<Task> act = async () => await _service.GetScaleRules(CancellationToken.None);

            await act.Should().ThrowAsync<NotImplementedException>();
        }
    }
}
