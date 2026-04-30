using Azure.Messaging.ServiceBus.Administration;
using Azure.ResourceManager.AppContainers.Models;

namespace aca_scaling_api.Interfaces
{
    public class ScaleRules
    {        
        public ScaleRules(ContainerAppScale containerAppScale)
        {
        int? minReplicas = containerAppScale.MinReplicas;
        int? maxReplicas = containerAppScale.MaxReplicas;
        int? coolDownPeriod = containerAppScale.CooldownPeriod;
                
        }

        public int? MinReplicas { get; set; } = 0;
        public int? MaxReplicas { get; set; } = 0;
        public int? CoolDownPeriod { get; set; } = 0;

    }
}

