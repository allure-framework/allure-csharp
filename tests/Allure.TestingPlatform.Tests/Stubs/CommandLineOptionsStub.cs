using Microsoft.Testing.Platform.CommandLine;

namespace Allure.TestingPlatform.Tests.Stubs;

public class CommandLineOptionsStub : ICommandLineOptions
{
    public bool? IsAllureEnabled { get; set; } = default;

    public bool IsOptionSet(string optionName) => this.IsAllureEnabled.HasValue;

    public bool TryGetOptionArgumentList(string optionName, out string[] arguments)
    {
        arguments = this.IsAllureEnabled switch
        {
            null => [],
            true => ["on"],
            false => ["off"],
        };
        return this.IsAllureEnabled.HasValue;
    }
}