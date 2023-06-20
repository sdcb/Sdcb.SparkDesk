using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents the chat property for chat parameters.
/// </summary>
public class ChatRequestParameters
{
    /// <summary>
    /// Specifies the domain to access, must be set to "general".
    /// </summary>
    [JsonPropertyName("domain")]
    public string Domain { get; set; } = "general";

    /// <summary>
    /// Determines the randomness of the result. The higher the value, the higher the randomness, and the more likely the same question will get different answers. Range: [0,1], default: 0.5
    /// </summary>
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.5f;

    /// <summary>
    /// Specifies the maximum length of model response in tokens, Range: [1,4096], default: 2048.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 2048;
}
