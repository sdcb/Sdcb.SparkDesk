using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents the payload part of the API response.
/// </summary>
public class ApiPayload
{
    /// <summary>
    /// The choices part of the payload.
    /// </summary>
    [JsonPropertyName("choices")]
    public required Choice Choices { get; set; }

    /// <summary>
    /// The usage part of the payload (returned in the last result).
    /// </summary>
    [JsonPropertyName("usage")]
    public UsageResponse? Usage { get; set; }
}
