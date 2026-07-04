using System;
using System.Collections.Generic;

namespace Allure.Testing.Execution;

/// <summary>
/// Contains data that affects the execution of the sample project.
/// </summary>
public record class AllureSampleRunInput
{
    /// <summary>
    /// An object that defines the content of <c>allureConfig.json</c>. Some supported values
    /// include <c>System.Text.Json.Nodes.JsonObject</c> instances, dictionaries, and anonymous objects.
    /// </summary>
    public object? AllureConfiguration { get; init; } = null;

    /// <summary>
    /// The path to a directory used to write Allure results to and read them from.
    /// An absolute path is recommended.
    /// </summary>
    /// <remarks>
    /// When this value is set, the directory is caller-owned: the runner does not
    /// create, clear, or delete it. Existing files may be included in the returned
    /// results.
    /// </remarks>
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
    /// A timeout for the sample run. Can be set via the <c>ALLURE_TEST_TIMEOUT</c> environment
    /// variable. The default is 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(
        ResolveTestTimeoutSeconds()
    );

    public static AllureSampleRunInput Default { get; } = new();

    static int ResolveTestTimeoutSeconds()
    {
        var envVar = Environment.GetEnvironmentVariable("ALLURE_TEST_TIMEOUT");
        return int.TryParse(envVar, out int value)
            ? value
            : 30;
    }
}