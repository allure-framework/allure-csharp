using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.CommandLine;

namespace Allure.TestingPlatform;

public class AllureCliOptionsProvider() :
    AllureMtpExtensionBase(
        "07e2cc0c-5cc5-4d7e-aaf6-eb623676fb0b",
        "Allure.TestingPlatform options provider",
        "Allows configuring Allure via the CLI."
    ),
    ICommandLineOptionsProvider
{
    public IReadOnlyCollection<CommandLineOption> GetCommandLineOptions() => [
        new(
            "allure",
            """
            Determines if Allure must be enabled.
                on: Allure is enabled [default]
                off: Allure is disabled
            """,
            ArgumentArity.ExactlyOne,
            false
        )
    ];

    public Task<ValidationResult> ValidateOptionArgumentsAsync(CommandLineOption commandOption, string[] arguments)
    {
        if (commandOption.Name == "allure" && arguments[0] is not ("on" or "off"))
        {
            return ValidationResult.InvalidTask("the value must be 'on' of 'off'");
        }

        return ValidationResult.ValidTask;
    }

    public Task<ValidationResult> ValidateCommandLineOptionsAsync(ICommandLineOptions commandLineOptions)
    {
        return ValidationResult.ValidTask;
    }

    public static bool IsAllureEnabled(ICommandLineOptions options) =>
        !options.TryGetOptionArgumentList("allure", out var values)
            || values[0] == "on";
}