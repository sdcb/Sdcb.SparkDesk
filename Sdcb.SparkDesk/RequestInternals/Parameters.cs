using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk.RequestInternals;

/// <summary>
/// Represents the chat parameters for the API request.
/// </summary>
internal class Parameters
{
    /// <summary>
    /// Contains chat-related properties such as domain, temperature, max tokens, and so on.
    /// </summary>
    [JsonPropertyName("chat")]
    public required ChatRequestParametersInternal Chat { get; set; }
}
