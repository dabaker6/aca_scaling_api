using aca_scaling_api.Configuration;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.AppContainers;
using Microsoft.Extensions.Options;

namespace aca_scaling_api.Services.ContainerApps
{
    public class ContainerAppsService : IContainerAppsService
    {
        //settings

        //get replica count
        //get configuration
        //endpoints getconfig, get replica count, service bus - get queue length
        private readonly ContainerAppsSettings _settings;
        private readonly ArmClient _armClient;        
        private ContainerAppRevisionResource? _activeRevision;
        public ContainerAppsService(IOptions<ContainerAppsSettings> settings, ArmClient armClient)
        {
            _settings = settings.Value;
            _armClient = armClient;
        }
        public async Task<int> GetReplicaCount(string revisionName, CancellationToken cancellationToken)
        {            
            return _activeRevision?.GetContainerAppReplicas().Count() ?? 0;
        }

        public async Task<string?> GetRevisionName(CancellationToken cancellationToken = default)
        {
            await GetActiveRevision(cancellationToken);

            return _activeRevision?.Data.Name ?? throw new InvalidOperationException("No active revision found.");
        }

        public async Task GetScaleRules(CancellationToken cancellationToken)
        {
            //ContainerAppResource containerAppResource = await GetContainerApp(cancellationToken);

            //var scale = containerAppResource.Data.Template.Scale;

            throw new NotImplementedException();
        }

        private async Task GetActiveRevision(CancellationToken cancellationToken)
        {            
            ContainerAppResource containerAppResource = await GetContainerApp(cancellationToken);
            ContainerAppRevisionCollection containerAppRevisionCollection = containerAppResource.GetContainerAppRevisions();

            await foreach (var revision in containerAppRevisionCollection.GetAllAsync(null, cancellationToken))
            {
                if (revision.Data.IsActive ?? false)
                {
                    _activeRevision = revision;
                    return;
                }
            }            
        }

        private async Task<ContainerAppResource> GetContainerApp(CancellationToken cancellationToken)
        {
            ResourceIdentifier id = ContainerAppResource.CreateResourceIdentifier(_settings.SubscriptionId, _settings.ResourceGroup, _settings.ContainerAppName);
            ContainerAppResource containerAppResource = _armClient.GetContainerAppResource(id);
            return await containerAppResource.GetAsync(cancellationToken);
        }
    }
}
