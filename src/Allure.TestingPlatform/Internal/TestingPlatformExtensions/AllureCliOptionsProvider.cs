using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.TestingPlatform.Functions;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.CommandLine;

namespace Allure.TestingPlatform.Internal.TestingPlatformExtensions;

public class AllureCliOptionsProvider() : ICommandLineOptionsProvider
{
    public string Uid => "07e2cc0c-5cc5-4d7e-aaf6-eb623676fb0b";

    public string Version => TestingPlatformFunctions.CurrentPackageVersion;

    public string DisplayName => "Allure.TestingPlatform options provider";

    public string Description => "Allows configuring Allure via the CLI.";

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

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
}