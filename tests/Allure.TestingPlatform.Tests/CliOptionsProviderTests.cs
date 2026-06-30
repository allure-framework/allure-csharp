using System.Xml.Linq;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Extensions.CommandLine;

namespace Allure.TestingPlatform.Tests;

public class CliOptionsProviderTests
{
    readonly AllureCliOptionsProvider provider = new();

    [Test]
    public async Task ShouldBeEnabled()
    {
        await Assert.That(this.provider.IsEnabledAsync()).IsTrue();
    }

    [Test]
    public async Task ShouldHaveNameDescriptionAndVersion()
    {
        await Assert.That(this.provider)
            .Member(p => p.Uid, v => v.IsEqualTo("07e2cc0c-5cc5-4d7e-aaf6-eb623676fb0b"))
            .And.Member(p => p.DisplayName, v => v.IsEqualTo("Allure.TestingPlatform options provider"))
            .And.Member(p => p.Description, v => v.IsEqualTo("Allows configuring Allure via the CLI."));
    }

    [Test]
    public async Task ShouldHaveVersionFromPackageMetadata()
    {
        using var stream = File.OpenRead("Directory.Build.props");
        var props = await XDocument.LoadAsync(stream, default, CancellationToken.None);
        var version = props.Root
            .Elements("PropertyGroup")
            .Select(e => e.Element("Version"))
            .First(e => e is not null)
            .Value;

        await Assert.That(this.provider.Version).IsEqualTo(version);
    }

    [Test]
    public async Task ShouldExposeAllureToggleOption()
    {
        var option = await Assert.That(this.provider.GetCommandLineOptions())
            .HasSingleItem(o => o.Name == "allure");

        await Assert.That(option.Arity).IsEqualTo(ArgumentArity.ExactlyOne);
        await Assert.That(option.IsHidden).IsFalse();
        await Assert.That(option.Description)
            .Contains("on")
            .And.Contains("off");
    }

    [Test]
    public async Task ShouldExposeAllureWatchdogToggleOption()
    {
        var option = await Assert.That(this.provider.GetCommandLineOptions())
            .HasSingleItem(o => o.Name == "allure-watchdog");

        await Assert.That(option.Arity).IsEqualTo(ArgumentArity.ExactlyOne);
        await Assert.That(option.IsHidden).IsFalse();
        await Assert.That(option.Description)
            .Contains("on")
            .And.Contains("off")
            .And.Contains("crashes");
    }

    [Test]
    [Arguments("on")]
    [Arguments("off")]
    public async Task ShouldAcceptAllureToggleValues(string value)
    {
        var option = await Assert.That(this.provider.GetCommandLineOptions())
            .HasSingleItem(o => o.Name == "allure");

        var result = await this.provider.ValidateOptionArgumentsAsync(option, [value]);

        await Assert.That(result.IsValid).IsTrue();
        await Assert.That(result.ErrorMessage).IsNull();
    }

    [Test]
    [Arguments("true")]
    [Arguments("false")]
    [Arguments("enabled")]
    [Arguments("")]
    public async Task ShouldRejectInvalidAllureToggleValues(string value)
    {
        var option = await Assert.That(this.provider.GetCommandLineOptions())
            .HasSingleItem(o => o.Name == "allure");

        var result = await this.provider.ValidateOptionArgumentsAsync(option, [value]);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.ErrorMessage).IsEqualTo("the value must be 'on' or 'off'");
    }

    [Test]
    [Arguments("on")]
    [Arguments("off")]
    public async Task ShouldAcceptAllureWatchdogToggleValues(string value)
    {
        var option = await Assert.That(this.provider.GetCommandLineOptions())
            .HasSingleItem(o => o.Name == "allure-watchdog");

        var result = await this.provider.ValidateOptionArgumentsAsync(option, [value]);

        await Assert.That(result.IsValid).IsTrue();
        await Assert.That(result.ErrorMessage).IsNull();
    }

    [Test]
    [Arguments("true")]
    [Arguments("false")]
    [Arguments("enabled")]
    [Arguments("")]
    public async Task ShouldRejectInvalidAllureWatchdogToggleValues(string value)
    {
        var option = await Assert.That(this.provider.GetCommandLineOptions())
            .HasSingleItem(o => o.Name == "allure-watchdog");

        var result = await this.provider.ValidateOptionArgumentsAsync(option, [value]);

        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.ErrorMessage).IsEqualTo("the value must be 'on' or 'off'");
    }

    [Test]
    public async Task ShouldIgnoreValidationForOtherOptions()
    {
        CommandLineOption option = new(
            "not-allure",
            "Unrelated option",
            ArgumentArity.ExactlyOne,
            false
        );

        var result = await this.provider.ValidateOptionArgumentsAsync(option, ["whatever"]);

        await Assert.That(result.IsValid).IsTrue();
        await Assert.That(result.ErrorMessage).IsNull();
    }

    [Test]
    public async Task ShouldAcceptAnyCommandLineOptionSet()
    {
        var result = await this.provider.ValidateCommandLineOptionsAsync(
            new CommandLineOptionsStub(("allure", ["invalid"]))
        );

        await Assert.That(result.IsValid).IsTrue();
        await Assert.That(result.ErrorMessage).IsNull();
    }

    [Test]
    public async Task ShouldReadAllureToggleValue()
    {
        await Assert.That(AllureCliOptionsProvider.GetAllureToggleValue(new CommandLineOptionsStub())).IsNull();
        await Assert.That(AllureCliOptionsProvider.GetAllureToggleValue(
            new CommandLineOptionsStub(("allure", ["on"]))
        )).IsTrue();
        await Assert.That(AllureCliOptionsProvider.GetAllureToggleValue(
            new CommandLineOptionsStub(("allure", ["off"]))
        )).IsFalse();
    }

    [Test]
    public async Task ShouldReadAllureWatchdogToggleValue()
    {
        await Assert.That(AllureCliOptionsProvider.GetWatchdogToggleValue(new CommandLineOptionsStub())).IsNull();
        await Assert.That(AllureCliOptionsProvider.GetWatchdogToggleValue(
            new CommandLineOptionsStub(("allure-watchdog", ["on"]))
        )).IsTrue();
        await Assert.That(AllureCliOptionsProvider.GetWatchdogToggleValue(
            new CommandLineOptionsStub(("allure-watchdog", ["off"]))
        )).IsFalse();
    }

    [Test]
    public async Task ShouldIgnoreNonAllureOptionsWhenReadingToggleValue()
    {
        var options = new CommandLineOptionsStub(("other", ["off"]));

        await Assert.That(AllureCliOptionsProvider.GetAllureToggleValue(options)).IsNull();
        await Assert.That(AllureCliOptionsProvider.GetWatchdogToggleValue(options)).IsNull();
        await Assert.That(AllureCliOptionsProvider.IsAllureEnabled(options)).IsTrue();
    }

    [Test]
    [Arguments(null, true)]
    [Arguments("on", true)]
    [Arguments("off", false)]
    public async Task ShouldReadAllureEnabledValue(string value, bool expected)
    {
        var options = value is null
            ? new CommandLineOptionsStub()
            : new CommandLineOptionsStub(("allure", [value]));

        await Assert.That(AllureCliOptionsProvider.IsAllureEnabled(options)).IsEqualTo(expected);
    }

    class CommandLineOptionsStub(params (string Name, string[] Values)[] options) : ICommandLineOptions
    {
        readonly Dictionary<string, string[]> options = options.ToDictionary(
            option => option.Name,
            option => option.Values
        );

        public bool IsOptionSet(string optionName) => this.options.ContainsKey(optionName);

        public bool TryGetOptionArgumentList(string optionName, out string[] arguments)
        {
            if (this.options.TryGetValue(optionName, out arguments))
            {
                return true;
            }

            arguments = [];
            return false;
        }
    }
}
