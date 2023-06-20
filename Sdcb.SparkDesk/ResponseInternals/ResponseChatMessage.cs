using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk.ResponseInternals;

/// <summary>
/// Represents a text object in the choices part of the API payload.
/// </summary>
internal class ResponseChatMessage
{
    /// <summary>
    /// The content of AI's answer.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    /// <summary>
    /// Role identifier, fixed as "assistant", indicating the role is AI.
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    /// <summary>
    /// Result sequence number, values are [0, 10]; currently a reserved field, developers can ignore it.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }
}
