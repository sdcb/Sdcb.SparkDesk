using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents a text object containing the role and content of a message.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Specifies the role of the message. Can be "user" for a user's question or "assistant" for an AI response.
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    /// <summary>
    /// The content of the message for the user or the AI. Note that the total tokens of all content properties must be within 8192 tokens.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    public static ChatMessage FromUser(string message) => new() { Role = "user", Content = message };
    public static ChatMessage FromAssistant(string message) => new() { Role = "assistant", Content = message };
}
