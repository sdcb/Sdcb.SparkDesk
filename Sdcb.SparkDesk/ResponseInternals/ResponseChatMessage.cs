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

    /// <summary>
    /// Gets or sets the type of content in the message. (e.g., "text")
    /// </summary>
    [JsonPropertyName("content_type")]
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets or sets the function call associated with the message.
    /// </summary>
    [JsonPropertyName("function_call")]
    public FunctionCall? FunctionCall { get; init; }
}

/// <summary>
/// Represents a function call with its arguments and name.
/// </summary>
public record FunctionCall
{
    /// <summary>
    /// Gets or sets the JSON representation of the arguments for the function call.
    /// </summary>
    [JsonPropertyName("arguments")]
    public required string Arguments { get; init; }

    /// <summary>
    /// Gets or sets the name of the function to be called.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}
