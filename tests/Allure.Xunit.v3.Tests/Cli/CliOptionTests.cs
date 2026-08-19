using Allure.Testing;

namespace Allure.Xunit.v3.Tests.Cli;

class CliOptionTests
{
    [Test]
    public async Task AllureOffShouldNotWriteResults(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.CliOptions, new()
        {
            ProcessArguments = ["--", "--allure", "off"],
        }, token);

        await Assert.That(results.TestResults).Count().IsEqualTo(0);
        await Assert.That(results.Containers).Count().IsEqualTo(0);
        await Assert.That(results.Globals).Count().IsEqualTo(0);
    }

    [Test]
    public async Task AllureOnShouldWriteResults(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.CliOptions, new()
        {
            ProcessArguments = ["--", "--allure", "on"],
        }, token);

        await Assert.That(results.TestResults).Count().IsEqualTo(6);
    }

    [Test]
    public async Task AllureResultsDirectoryShouldOverrideConfigurationDirectory(CancellationToken token)
    {
        using var cliDirectory = new TempDirectory("allure-cli-results-");

        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.CliOptions, new()
        {
            ProcessArguments = ["--", "--allure-results-directory", cliDirectory.Path],
        }, token);

        var cliResults = await AllureSampleRunner.ReadAllureResults(cliDirectory.Directory, token);

        await Assert.That(results.TestResults).Count().IsEqualTo(0);
        await Assert.That(results.Containers).Count().IsEqualTo(0);
        await Assert.That(results.Globals).Count().IsEqualTo(0);

        await Assert.That(cliResults.TestResults).Count().IsEqualTo(6);
    }

    [Test]
    public async Task AllureWatchdogOffShouldNotCreateGlobalsOnCrash(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(
            Lifecycle.AllureSampleRegistry.CrashingProcess,
            new()
            {
                ProcessArguments = ["--", "--allure-watchdog", "off"],
            },
            token
        );

        await Assert.That(results.Globals).Count().IsEqualTo(0);
    }

    [Test]
    public async Task XunitFilterMethodShouldBePreserved(CancellationToken token)
    {
        var selectedTest = "Allure.Xunit.v3.Tests.Samples.Cli.CliOptions.TestClass.FirstTest";
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.CliOptions, new()
        {
            ProcessArguments = ["--", "--filter-method", selectedTest],
        }, token);

        await Assert.That(results).HasSingleTestResult(selectedTest);
    }

    sealed class TempDirectory(string prefix) : IDisposable
    {
        public DirectoryInfo Directory { get; } = System.IO.Directory.CreateTempSubdirectory(prefix);

        public string Path => Directory.FullName;

        public void Dispose()
        {
            Directory.Refresh();
            if (Directory.Exists)
            {
                Directory.Delete(recursive: true);
            }
        }
    }
}
