using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents a text object containing the role and content of a message.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessage"/> class with the specified role and content.
    /// </summary>
    /// <param name="role">Specifies the role of the message. Can be "user" for a user's question or "assistant" for an AI response.</param>
    /// <param name="content">The content of the message for the user or the AI. Note that the total tokens of all content properties must be within 8192 tokens.</param>
    [SetsRequiredMembers]
    public ChatMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }

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

    /// <summary>
    /// Creates a new <see cref="ChatMessage"/> instance for a user with the specified message content.
    /// </summary>
    /// <param name="content">The text content of the user's message.</param>
    /// <returns>A new <see cref="ChatMessage"/> instance representing the user's message.</returns>
    public static ChatMessage FromUser(string content) => new("user", content);

    /// <summary>
    /// Creates a new  <see cref="ChatMessage"/> instance for an AI assistant with the specified message content.
    /// </summary>
    /// <param name="content">The text content of the AI assistant's message.</param>
    /// <returns>A new <see cref="ChatMessage"/> instance representing the AI assistant's message.</returns>
    public static ChatMessage FromAssistant(string content) => new("assistant", content);
}
