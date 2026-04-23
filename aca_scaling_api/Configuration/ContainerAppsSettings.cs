using System.ComponentModel.DataAnnotations;

namespace aca_scaling_api.Configuration
{
    public class ContainerAppsSettings
    {
        public const string SectionName = "ContainerApps";
        [Required]
        public string ResourceGroup { get; init; } = string.Empty;

        [Required]
        public string ContainerAppName { get; init; } = string.Empty;

        [Required]
        public string SubscriptionId { get; init; } = string.Empty;
    }
}
