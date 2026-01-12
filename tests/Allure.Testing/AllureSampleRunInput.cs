using System;
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
    /// The path to a directory used to write and read Allure results to/from. An absolute path
    /// is recommended.
    /// </summary>
    public string? AllureResultsDirectory { get; init; } = null;

    /// <summary>
    /// A dictionary that defines extra environment variables passed to
    /// <c>dotnet test</c> or <c>dotnet run</c>.
    /// </summary>
    public Dictionary<string, string> EnvironmentVariables { get; init; } = [];

    /// <summary>
    /// A list of extra CLI arguments passed to <c>dotnet test</c> or <c>dotnet run</c>.
    /// </summary>
    public List<string> ProcessArguments { get; init; } = [];

    /// <summary>
    /// A timeout of the sample run.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

    public static AllureSampleRunInput Default { get; } = new();
}