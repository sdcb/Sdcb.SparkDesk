using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk.RequestInternals;

/// <summary>
/// Represents a message, which contains a list of text objects.
/// </summary>
internal class Message
{
    /// <summary>
    /// A list of text objects representing the conversation between the user and the assistant. Each object contains a role and content property.
    /// </summary>
    [JsonPropertyName("text")]
    public required ChatMessage[] Text { get; set; }
}
