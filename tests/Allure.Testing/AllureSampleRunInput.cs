using System.Collections.Generic;

namespace Allure.Testing;

/// <summary>
/// Contains data that affects the execution of the sample project.
/// </summary>
public record class AllureSampleRunInput
{
    /// <summary>
    /// An object that defines the content of <c>allureConfig.json</c>. Some supported values
    /// of this parameter include <c>System.Text.Json.Nodes.JsonObject</c> instances, dictionaries,
    /// and anonymous objects.
    /// </summary>
    public object? AllureConfiguration { get; init; } = null;

    /// <summary>
    /// A dictionary that defines extra environment variables passed to
    /// <c>dotnet test</c> or <c>dotnet run</c>.
    /// </summary>
    public Dictionary<string, string> EnvironmentVariables { get; init; } = [];

    /// <summary>
    /// A list of extra CLI arguments passed to <c>dotnet test</c> or <c>dotnet run</c>.
    /// </summary>
    public List<string> ProcessArguments { get; init; } = [];

    public static AllureSampleRunInput Default { get; } = new();
}