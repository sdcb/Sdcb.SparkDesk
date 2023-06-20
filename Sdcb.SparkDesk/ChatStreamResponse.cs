using System.Linq;

namespace Sdcb.SparkDesk;

/// <summary>
/// Response struct for chat stream 
/// </summary>
public readonly record struct StreamedChatResponse
{
    /// <param name="Text">The chat stream response text</param>
    /// <param name="Usage">The text usage enumeration value</param>
    public StreamedChatResponse(string Text, TextUsage? Usage)
    {
        this.Text = Text;
        this.Usage = Usage;
    }

    /// <summary>
    /// The chat stream response text
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// The text usage enumeration value
    /// </summary>
    public TextUsage? Usage { get; }

    /// <summary>
    /// Implicit conversion from ChatStreamResponse to string
    /// </summary>
    /// <param name="r">The chat stream response to be converted</param>
    public static implicit operator string(StreamedChatResponse r) => r.Text;

    /// <summary>
    /// Returns the chat stream response text
    /// </summary>
    public override readonly string ToString() => Text;
}
