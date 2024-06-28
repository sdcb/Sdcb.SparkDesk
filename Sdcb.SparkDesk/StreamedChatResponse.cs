using Sdcb.SparkDesk.ResponseInternals;

namespace Sdcb.SparkDesk;

/// <summary>
/// Response struct for chat stream 
/// </summary>
public readonly record struct StreamedChatResponse
{
    /// <param name="text">The chat stream response text</param>
    /// <param name="usage">The text usage enumeration value</param>
    /// <param name="contentType">The content type of the chat stream response</param>
    /// <param name="functionCall">The function call associated with the chat stream response</param>
    public StreamedChatResponse(string text, TokensUsage? usage, string? contentType, FunctionCall? functionCall)
    {
        Text = text;
        Usage = usage;
        ContentType = contentType;
        FunctionCall = functionCall;
    }

    /// <summary>
    /// The chat stream response text
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the content type of the chat stream response. 
    /// This can be null if the content type is not specified.
    /// </summary>
    public string? ContentType { get; }

    /// <summary>
    /// Gets the function call associated with the chat stream response.
    /// This can be null if there is no function call associated.
    /// </summary>
    public FunctionCall? FunctionCall { get; }

    /// <summary>
    /// The text usage enumeration value
    /// </summary>
    public TokensUsage? Usage { get; }

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
