using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

/// <summary>
/// The main container for the API response.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// The header part of the response.
    /// </summary>
    [JsonPropertyName("header")]
    public required ResponseHeader Header { get; set; }

    /// <summary>
    /// The payload part of the response.
    /// </summary>
    [JsonPropertyName("payload")]
    public ApiPayload? Payload { get; set; }
}
