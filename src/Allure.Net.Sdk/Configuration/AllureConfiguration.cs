using System;
using System.Collections.Immutable;
using System.IO;

namespace Allure.Sdk.Configuration;

/// <summary>
/// Defines the common configuration used by an Allure runtime.
/// </summary>
public record class AllureConfiguration
{
    readonly string resultsDirectory =
        Path.Combine(Environment.CurrentDirectory, "allure-results");

    /// <summary>
    /// Gets the host name recorded in generated test results.
    /// </summary>
    public string Hostname { get; init; } = Environment.MachineName;

    /// <summary>
    /// Gets the absolute path of the directory where Allure result files are written.
    /// </summary>
    public string ResultsDirectory
    {
        get => this.resultsDirectory;
        init
        {
            this.resultsDirectory = Path.GetFullPath(value);
        }
    }

    /// <summary>
    /// Gets the link templates indexed by link type.
    /// </summary>
    public ImmutableDictionary<string, AllureLinkTemplate> LinkTemplates { get; init; } = [];

    /// <summary>
    /// Gets the exception type names that Allure treats as test failures.
    /// </summary>
    public ImmutableList<string> FailExceptions { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether generated JSON files are indented.
    /// </summary>
    public bool IndentOutput { get; init; } = false;

    /// <summary>
    /// Gets the labels applied to every test result.
    /// </summary>
    public ImmutableDictionary<string, string> GlobalLabels { get; init; } = [];

    /// <summary>
    /// Gets the assembly-qualified name of the runtime registration hook type.
    /// </summary>
    public string? RegistrationHook { get; init; }
}
