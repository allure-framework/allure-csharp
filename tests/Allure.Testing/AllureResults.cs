using System;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace Allure.Testing;

/// <summary>
/// Contains the output of the Allure integration that's been run against the sample project.
/// </summary>
/// <param name="TestResults">The parsed content of the *-result.json files.</param>
/// <param name="Containers">The parsed content of the *-container.json files.</param>
/// <param name="Attachments">
/// A dictionary that maps Allure attachment names to their content.
/// </param>
public record class AllureResults(
    ImmutableArray<JsonObject> TestResults,
    ImmutableArray<JsonObject> Containers,
    ImmutableDictionary<string, ReadOnlyMemory<byte>> Attachments
);
