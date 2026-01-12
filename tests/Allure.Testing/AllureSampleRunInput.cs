using System.Collections.Generic;

namespace Allure.Testing;

/// <summary>
/// Contains data that affects the execution of the sample project.
/// </summary>
/// <param name="ProcessArguments">
/// A list of extra CLI arguments passed to <c>dotnet test</c> or <c>dotnet run</c>.
/// </param>
/// <param name="AllureConfiguration">
/// An object that defines the content of <c>allureConfig.json</c>. Some supported values
/// of this parameter include <c>System.Text.Json.Nodes.JsonObject</c> instances, dictionaries, and
/// anonymous objects.
/// </param>
/// <param name="EnvironmentVariables">
/// A dictionary that defines extra environment variables passed to
/// <c>dotnet test</c> or <c>dotnet run</c>.
/// </param>
public record class AllureSampleRunInput(
    List<string> ProcessArguments,
    object? AllureConfiguration,
    Dictionary<string, string> EnvironmentVariables
)
{
    public static AllureSampleRunInput Default { get; } = new([], null, []);
}