using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

public class UsageResponse
{
    [JsonPropertyName("text")]
    public required TextUsage Text { get; set; }
}
