using Microsoft.Testing.Platform.CommandLine;

namespace Allure.TestingPlatform.Tests.Stubs;

public class CommandLineOptionsStub : ICommandLineOptions
{
    readonly Dictionary<string, string[]> values = [];

    public bool IsAllureEnabled
    {
        set
        {
            this.values["allure"] = [value ? "on" : "off"];
        }
    }

    public string ResultsDirectory
    {
        set
        {
            this.values["allure-results-directory"] = [value];
        }
    }

    public bool IsOptionSet(string optionName) => this.values.ContainsKey(optionName);

    public bool TryGetOptionArgumentList(string optionName, out string[] arguments) =>
        this.values.TryGetValue(optionName, out arguments);
}