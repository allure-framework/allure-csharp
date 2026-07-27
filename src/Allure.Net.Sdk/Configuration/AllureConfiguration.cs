using System;
using System.Collections.Immutable;
using System.IO;

namespace Allure.Sdk.Configuration;

public record class AllureConfiguration
{
    readonly string resultsDirectory =
        Path.Combine(Environment.CurrentDirectory, "allure-results");

    public string Hostname { get; init; } = Environment.MachineName;

    public string ResultsDirectory
    {
        get => this.resultsDirectory;
        init
        {
            this.resultsDirectory = Path.GetFullPath(value);
        }
    }

    public ImmutableDictionary<string, AllureLinkTemplate> LinkTemplates { get; init; } = [];

    public ImmutableList<string> FailExceptions { get; init; } = [];

    public bool IndentOutput { get; init; } = false;

    public ImmutableDictionary<string, string> GlobalLabels { get; init; } = [];

    public string? RegistrationHook { get; init; }
}
