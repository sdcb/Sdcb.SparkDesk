using System;
using System.Collections.Generic;
using System.Text;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents the configuration settings for a model.
/// </summary>
/// <param name="DisplayName">The display name of the model.</param>
/// <param name="Domain">The specific domain area for accessing the model. Valid values are: "general", "generalv2", "generalv3", "generalv3.5", "4.0Ultra".</param>
/// <param name="AddressPart">The address part specifying the version for the API endpoint. Valid formats are akin to "wss://spark-api.xf-yun.com/{AddressPart}/chat" with values being "v1.1", "v2.1", "v3.1", "v3.5", "v4.0".</param>
public record ModelVersion(string DisplayName, string Domain, string AddressPart)
{
    /// <summary>
    /// The 'Lite' version of the model, suitable for basic use cases.
    /// </summary>
    public static ModelVersion Lite { get; } = new("Lite", "general", "v1.1");

    /// <summary>
    /// The 'V2' version of the model, representing an enhancement over the 'Lite' version in the "generalv2" domain.
    /// </summary>
    public static ModelVersion V2_0 { get; } = new("V2", "generalv2", "v2.1");

    /// <summary>
    /// The 'Pro' version of the model, designed for more professional usage in the "generalv3" domain.
    /// </summary>
    public static ModelVersion Pro { get; } = new("Pro", "generalv3", "v3.1");

    /// <summary>
    /// The 'Max' version offering advanced features in the "generalv3.5" domain.
    /// </summary>
    public static ModelVersion Max { get; } = new("Max", "generalv3.5", "v3.5");

    /// <summary>
    /// The '4.0 Ultra' version, which is the most advanced model configuration available in the "4.0Ultra" domain.
    /// </summary>
    public static ModelVersion V4_0_Ultra { get; } = new("4.0 Ultra", "4.0Ultra", "v4.0");

    /// <summary>
    /// Gets the WebSocket URL constructed using the specified address part.
    /// </summary>
    public virtual string WebsocketUrl => $"wss://spark-api.xf-yun.com/{AddressPart}/chat";

    /// <summary>
    /// Returns the display name of the model.
    /// </summary>
    public override string ToString() => DisplayName;
}