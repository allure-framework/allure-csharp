using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.TestingPlatform.Functions;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.CommandLine;

namespace Allure.TestingPlatform.Sdk.TestingPlatformExtensions;

/// <summary>
/// Provides Allure command-line options for Microsoft Testing Platform.
/// </summary>
public class AllureCliOptionsProvider() : ICommandLineOptionsProvider
{
    /// <inheritdoc />
    public string Uid => "07e2cc0c-5cc5-4d7e-aaf6-eb623676fb0b";

    /// <inheritdoc />
    public string Version => TestingPlatformFunctions.CurrentPackageVersion;

    /// <inheritdoc />
    public string DisplayName => "Allure.TestingPlatform options provider";

    /// <inheritdoc />
    public string Description => "Allows configuring Allure via the CLI.";

    /// <inheritdoc />
    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    /// <inheritdoc />
    public IReadOnlyCollection<CommandLineOption> GetCommandLineOptions() => [
        new(
            "allure",
            """
            Determines whether Allure is enabled.
                on: Allure is enabled [default]
                off: Allure is disabled
            """,
            ArgumentArity.ExactlyOne,
            false
        ),
        new(
            "allure-watchdog",
            """
            Determines whether the Allure watchdog is enabled. The watchdog records a global
            error in the report if the test process crashes.
                on: The watchdog is enabled [default]
                off: The watchdog is disabled
            """,
            ArgumentArity.ExactlyOne,
            false
        ),
        new(
            "allure-results-directory",
            """
            Specifies the output directory where Allure result files will be written.
            """,
            ArgumentArity.ExactlyOne,
            false
        ),
    ];

    /// <inheritdoc />
    public Task<ValidationResult> ValidateOptionArgumentsAsync(
        CommandLineOption commandOption,
        string[] arguments
    ) =>
        commandOption.Name switch
        {
            "allure" or "allure-watchdog" when arguments[0] is not ("on" or "off") =>
                ValidationResult.InvalidTask("the value must be 'on' or 'off'"),

            "allure-results-directory" when arguments[0] is null or { Length: 0 } =>
                ValidationResult.InvalidTask("the value cannot be empty"),

            _ => ValidationResult.ValidTask,
        };

    /// <inheritdoc />
    public Task<ValidationResult> ValidateCommandLineOptionsAsync(
        ICommandLineOptions commandLineOptions
    ) =>
        ValidationResult.ValidTask;

    /// <summary>
    /// Gets the configured value of the <c>--allure</c> toggle.
    /// </summary>
    public static bool? GetAllureToggleValue(ICommandLineOptions options) =>
        options.TryGetOptionArgumentList("allure", out var values)
            ? values[0] == "on"
            : null;

    /// <summary>
    /// Gets whether Allure is enabled by command-line options.
    /// </summary>
    public static bool IsAllureEnabled(ICommandLineOptions options) =>
        !options.TryGetOptionArgumentList("allure", out var values)
            || values[0] == "on";

    /// <summary>
    /// Gets the configured value of the <c>--allure-watchdog</c> toggle.
    /// </summary>
    public static bool? GetWatchdogToggleValue(ICommandLineOptions options) =>
        options.TryGetOptionArgumentList("allure-watchdog", out var values)
            ? values[0] == "on"
            : null;

    /// <summary>
    /// Gets the value of the <c>--allure-results-directory</c> option.
    /// </summary>
    public static string? GetResultsDirectoryValue(ICommandLineOptions options) =>
        options.TryGetOptionArgumentList("allure-results-directory", out var values)
            ? values[0]
            : null;
}
