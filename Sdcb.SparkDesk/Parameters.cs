using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents the chat parameters for the API request.
/// </summary>
public class Parameters
{
    /// <summary>
    /// Contains chat-related properties such as domain, temperature, max tokens, and so on.
    /// </summary>
    [JsonPropertyName("chat")]
    public ChatRequestParameters Chat { get; set; } = new ChatRequestParameters();
}
