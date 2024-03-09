using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sdcb.SparkDesk;

/// <summary>
/// Represents a function, including its name, description, and parameters.
/// </summary>
public record FunctionDef
{
    /// <summary>
    /// Gets the function name which will be returned after it is triggered by a user input.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets a detailed description of the function.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the parameter list for the function.
    /// </summary>
    public required IReadOnlyList<FunctionParametersDef> Parameters { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionDef"/> class.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="description">The description of the function.</param>
    /// <param name="parameters">The parameters of the function.</param>
    [SetsRequiredMembers]
    public FunctionDef(string name, string description, IReadOnlyList<FunctionParametersDef> parameters)
    {
        Name = name;
        Description = description;
        Parameters = parameters;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionDef"/> class.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    public FunctionDef(string name)
    {
        Name = name;
    }
}

/// <summary>
/// Represents the properties of a function parameter, including its type, description, and whether it is required.
/// </summary>
public record FunctionParametersDef
{
    /// <summary>
    /// Gets or init the name of the parameter.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the data type description of the parameter, default: <c>string</c>.
    /// </summary>
    public required string Type { get; init; } = "string";

    /// <summary>
    /// Gets a detailed description of the parameter.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Indicates whether the parameter is required.
    /// </summary>
    public required bool Required { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionParametersDef"/> class.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="type">The data type of the parameter.</param>
    /// <param name="description">The description of the parameter.</param>
    /// <param name="required">Indicates whether the parameter is required. Default is true.</param>
    [SetsRequiredMembers]
    public FunctionParametersDef(string name, string type, string description, bool required = true)
    {
        Name = name;
        Type = type;
        Description = description;
        Required = required;
    }
}