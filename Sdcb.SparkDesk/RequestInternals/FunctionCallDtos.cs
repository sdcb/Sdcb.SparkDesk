using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Sdcb.SparkDesk.RequestInternals;

/// <summary>
/// Represents the payload for the SparkDesk Function Call request.
/// </summary>
internal record FunctionCallDto
{
    /// <summary>
    /// Gets the list of functions with their details.
    /// </summary>
    [JsonPropertyName("text")]
    public required IList<FunctionDto> Text { get; init; }

    public static FunctionCallDto FromFunctionDefs(IEnumerable<FunctionDef> functionDefs)
    {
        return new FunctionCallDto
        {
            Text = functionDefs.Select(f => new FunctionDto
            {
                Name = f.Name,
                Description = f.Description,
                Parameters = new FunctionParametersDto
                {
                    Type = "object",
                    Properties = f.Parameters.ToDictionary(p => p.Name, p => new ParameterPropertyDto
                    {
                        Type = p.Type,
                        Description = p.Description
                    }),
                    Required = f.Parameters.Where(p => p.Required).Select(p => p.Name).ToList()
                }
            }).ToList()
        };
    }
}

/// <summary>
/// Represents a function, including its name, description, and parameters.
/// </summary>
internal record FunctionDto
{
    /// <summary>
    /// Gets the function name which will be returned after it is triggered by a user input.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets a detailed description of the function.
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    /// <summary>
    /// Gets the parameter list for the function.
    /// </summary>
    [JsonPropertyName("parameters")]
    public required FunctionParametersDto Parameters { get; init; }
}

/// <summary>
/// Represents the parameter structure including its type, properties and required fields.
/// </summary>
internal record FunctionParametersDto
{
    /// <summary>
    /// Gets the parameter type.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Gets the parameter information descriptions.
    /// </summary>
    [JsonPropertyName("properties")]
    public required IDictionary<string, ParameterPropertyDto> Properties { get; init; }

    /// <summary>
    /// Gets the list of required parameter field names.
    /// </summary>
    [JsonPropertyName("required")]
    public required IList<string> Required { get; init; }
}

/// <summary>
/// Represents the properties of a function parameter, including its type and description.
/// </summary>
internal record ParameterPropertyDto
{
    /// <summary>
    /// Gets the data type description of the parameter.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// Gets a detailed description of the parameter.
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; init; }
}