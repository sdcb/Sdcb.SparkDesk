using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents the header which contains app_id and uid.
/// </summary>
public class Header
{
    /// <summary>
    /// The application appid, obtained from the open platform control panel.
    /// </summary>
    [JsonPropertyName("app_id")]
    public required string AppId { get; set; }

    /// <summary>
    /// The user's id, used to distinguish between different users. Not required.
    /// </summary>
    [JsonPropertyName("uid")]
    public string? Uid { get; set; }
}
