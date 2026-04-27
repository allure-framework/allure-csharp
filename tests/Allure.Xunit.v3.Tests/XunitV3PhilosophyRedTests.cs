using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.v3.Tests;

class XunitV3PhilosophyRedTests
{
    [Test]
    public async Task CustomReporterShouldOnlyWorkWithInProcessExecution()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SetAllureIdFromTest);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
    }

    [Test]
    public async Task DotnetTestStyleExecutionShouldBeOutOfScopeForCustomReporter()
    {
        var csproj = await File.ReadAllTextAsync(
            Path.Combine(
                FindRepoRoot().FullName,
                "src",
                "Allure.Xunit.v3",
                "Allure.Xunit.v3.csproj"
            )
        );

        await Assert.That(csproj.Contains("xunit.runner.visualstudio")).IsFalse();
        await Assert.That(csproj.Contains("Microsoft.NET.Test.Sdk")).IsFalse();
    }

    [Test]
    public async Task ReporterPackageShouldNotPinOrLeakXunitRuntimeToConsumers()
    {
        var csproj = await File.ReadAllTextAsync(
            Path.Combine(
                FindRepoRoot().FullName,
                "src",
                "Allure.Xunit.v3",
                "Allure.Xunit.v3.csproj"
            )
        );

        await Assert.That(csproj.Contains("xunit.v3\" Version")).IsFalse();
        await Assert.That(csproj.Contains("PrivateAssets=\"all\"")).IsTrue();
    }

    [Test]
    public async Task MetadataMustBeCorrelatedFromStartingMessagesThroughCompletion()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.XunitDisplayNameOnFact);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["name"]).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task MinimalMvpShouldEmitPassedFailedSkippedAllureResults()
    {
        var passed = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SetAllureIdFromTest);
        var failed = await AllureSampleRunner.RunAsync(AllureSampleRegistry.TheoryWithThrowingMemberData);

        await Assert.That((string)passed.TestResults[0]["status"]).IsEqualTo("passed");
        await Assert.That((string)failed.TestResults[0]["status"]).IsEqualTo("broken");
    }

    static DirectoryInfo FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var candidate = Path.Combine(
                current.FullName,
                "src",
                "Allure.Xunit.v3",
                "Allure.Xunit.v3.csproj"
            );

            if (File.Exists(candidate))
            {
                return current;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Cannot locate repository root for tests.");
    }
}
