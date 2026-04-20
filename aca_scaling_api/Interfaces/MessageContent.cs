namespace aca_scaling_api.Interfaces
{
    public sealed record MessageContent
    {
        public string workId { get; set; } = string.Empty;
        public string jobId { get; set; } = string.Empty;
    }
}
