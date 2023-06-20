using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk.ResponseInternals;

internal class UsageResponse
{
    [JsonPropertyName("text")]
    public required TokensUsage Text { get; set; }
}
