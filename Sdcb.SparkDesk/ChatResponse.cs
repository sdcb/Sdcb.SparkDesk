using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents ChatResponse object that contains an array of <see cref="StreamedChatResponse"/> objects.
/// </summary>
public readonly record struct ChatResponse
{
    /// <summary>
    /// Initializes a new instance of the ChatResponse class with an array of <see cref="StreamedChatResponse"/> objects.
    /// </summary>
    /// <param name="streamedResponses">An array of <see cref="StreamedChatResponse"/> objects.</param>
    public ChatResponse(IReadOnlyList<StreamedChatResponse> streamedResponses)
    {
        if (streamedResponses.Count == 0)
        {
            throw new ArgumentException("The StreamedResponses array must not be empty.", nameof(streamedResponses));
        }

        StreamedResponses = streamedResponses;
    }

    /// <summary>
    /// Gets the concatenated text of all <see cref="StreamedChatResponse"/> objects in the <see cref="StreamedChatResponse"/> array.
    /// </summary>
    /// <returns>The concatenated text of all <see cref="StreamedChatResponse"/> objects.</returns>
    public readonly string Text => string.Concat(StreamedResponses.Select(x => x.Text));

    /// <summary>
    /// Gets the <see cref="TokensUsage"/> of the last <see cref="StreamedChatResponse"/> object in the StreamedResponses array.
    /// </summary>
    /// <returns>The <see cref="TokensUsage"/> of the last <see cref="StreamedChatResponse"/> object.</returns>
    public readonly TokensUsage Usage => StreamedResponses[StreamedResponses.Count - 1].Usage!;

    /// <summary>
    /// Converts a ChatResponse object to its string representation.
    /// </summary>
    /// <param name="r">The ChatResponse object to convert.</param>
    public static implicit operator string(ChatResponse r) => r.Text;

    /// <summary>
    /// Returns the concatenated text of all <see cref="StreamedChatResponse"/> objects in the StreamedResponses array.
    /// </summary>
    /// <returns>The concatenated text of all <see cref="StreamedChatResponse"/> objects.</returns>
    public override readonly string ToString() => Text;

    /// <summary>
    /// Gets an array of <see cref="StreamedChatResponse"/> objects.
    /// </summary>
    public readonly IReadOnlyList<StreamedChatResponse> StreamedResponses { get; }
}
