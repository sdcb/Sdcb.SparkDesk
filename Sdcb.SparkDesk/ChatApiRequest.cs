using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents the main structure of the JSON for the chat API request.
/// </summary>
public class ChatApiRequest
{
    /// <summary>
    /// Contains header information such as app_id and uid.
    /// </summary>
    [JsonPropertyName("header")]
    public required Header Header { get; set; }

    /// <summary>
    /// Contains parameter information for the chat domain, temperature, max tokens, etc.
    /// </summary>
    [JsonPropertyName("parameter")]
    public required Parameters Parameter { get; set; }

    /// <summary>
    /// Contains the payload information which includes a list of user and assistant messages.
    /// </summary>
    [JsonPropertyName("payload")]
    public required Payload Payload { get; set; }
}
