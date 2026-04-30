using System.ComponentModel.DataAnnotations;

namespace aca_scaling_api.Configuration
{
    public class ServiceBusSettings
    {
        public const string SectionName = "ServiceBus";
        [Required]
        public string FullyQualifiedNamespace { get; init; } = string.Empty;

        [Required]
        public string QueueName { get; init; } = string.Empty;
        
        [Required]
        public int ProcessingTime { get; init; }
    }
}
