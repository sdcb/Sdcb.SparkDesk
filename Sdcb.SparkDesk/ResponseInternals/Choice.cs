using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk.ResponseInternals;

/// <summary>
/// Represents the choices part of the API payload.
/// </summary>
internal class Choice
{
    /// <summary>
    /// Text response status, values are [0, 1, 2]; 0 represents the first text result; 1 represents the intermediate text result; 2 represents the last text result.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// The sequence number of the returned data, values are [0, 9999999].
    /// </summary>
    [JsonPropertyName("seq")]
    public int Seq { get; set; }

    /// <summary>
    /// An array of text objects.
    /// </summary>
    [JsonPropertyName("text")]
    public required ResponseChatMessage[] Text { get; set; }
}
