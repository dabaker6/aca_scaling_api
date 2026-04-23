namespace aca_scaling_api.Services.ContainerApps
{
    public interface IContainerAppsService
    {
        public Task<int> GetReplicaCount(string revisionName, CancellationToken cancellationToken);

        public Task GetScaleRules(CancellationToken cancellationToken);

        public Task<string> GetRevisionName(CancellationToken cancellationToken);
    }
}
