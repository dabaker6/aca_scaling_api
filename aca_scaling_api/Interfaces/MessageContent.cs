namespace aca_scaling_api.Interfaces
{
    public sealed record MessageContent
    {
        public string WorkId { get; set; } = string.Empty;
        public string JobId { get; set; } = string.Empty;

        public string CorrelationId {  get; set; } = string.Empty;
    }
}
