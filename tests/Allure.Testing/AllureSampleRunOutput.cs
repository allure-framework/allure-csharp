using System;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace Allure.Testing;

/// <summary>
/// Contains the results of a sample project's run.
/// </summary>
/// <param name="ExitCode">
/// The exit code of the sample test process (<c>dotnet test</c> or <c>dotnet run</c>).
/// </param>
/// <param name="StdOut">The standard output text.</param>
/// <param name="StdErr">The standard error text.</param>
/// <param name="AllureResults">The output produced by the Allure integration under test.</param>
public record class AllureSampleRunOutput(
    int ExitCode,
    string StdOut,
    string StdErr,
    AllureSampleRunOutput.AllureResultData AllureResults
)
{
    /// <summary>
    /// Contains the output of the Allure integration that's been run against the sample project.
    /// </summary>
    /// <param name="TestResults">The parsed content of the *-result.json files.</param>
    /// <param name="Containers">The parsed content of the *-container.json files.</param>
    /// <param name="Attachments">
    /// A dictionary that maps Allure attachment names to their content.
    /// </param>
    public record class AllureResultData(
        ImmutableArray<JsonObject> TestResults,
        ImmutableArray<JsonObject> Containers,
        ImmutableDictionary<string, ReadOnlyMemory<byte>> Attachments
    );
}
