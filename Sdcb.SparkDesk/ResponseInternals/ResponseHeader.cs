using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk.ResponseInternals;

/// <summary>
/// Represents the header part of the API response.
/// </summary>
internal class ResponseHeader
{
    /// <summary>
    /// Error code, 0 means normal, non-0 means error.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// Description of whether the session was successful.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; set; }

    /// <summary>
    /// The unique ID of the session, used for querying service side session logs by iFlytek technical staff. Please keep this field if you encounter a call error.
    /// </summary>
    [JsonPropertyName("sid")]
    public required string Sid { get; set; }

    /// <summary>
    /// Session status, values are [0, 1, 2]; 0 represents the first result; 1 represents the intermediate result; 2 represents the last result.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}
