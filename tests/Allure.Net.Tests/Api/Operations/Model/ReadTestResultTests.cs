using Allure.Abstractions;
using TUnit.Mocks.Assertions;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Operations.Model;

public class ReadTestResultTests : AllureApiTestsBase
{
    [Test]
    public async Task ReadTestResultReturnsEndpointValue()
    {
        Func<TestResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadTestResult(read);

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadTestResult<string>(read))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ReadTestResultThrowsWhenValueIsMissing()
    {
        Func<TestResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult<string>(read).Returns(false);

        await Assert.That(() => AllureInProcessApi.ReadTestResult(read))
            .Throws<InvalidOperationException>()
            .WithMessage("Cannot read test result: no test is currently running.");
    }

    [Test]
    public async Task ReadTestResultWithFallbackReturnsEndpointValue()
    {
        Func<TestResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadTestResult(read, "Fallback value");

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadTestResult<string>(read))
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task ReadTestResultWithFallbackReturnsFallbackWhenValueIsMissing()
    {
        Func<TestResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult<string>(read).Returns(false);

        var result = AllureInProcessApi.ReadTestResult(read, "Fallback value");

        await Assert.That(result).IsEqualTo("Fallback value");
    }

    [Test]
    public async Task ReadTestResultWithFactoryDoesNotInvokeFactoryWhenValueExists()
    {
        Func<TestResult, string> read = result => result.Name;
        var fallbackCalls = 0;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadTestResult(
            read,
            () => { fallbackCalls++; return "Fallback value"; }
        );

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(fallbackCalls).IsZero();
    }

    [Test]
    public async Task ReadTestResultWithFactoryInvokesFactoryWhenValueIsMissing()
    {
        Func<TestResult, string> read = result => result.Name;
        var fallbackCalls = 0;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult<string>(read).Returns(false);

        var result = AllureInProcessApi.ReadTestResult(
            read,
            () => { fallbackCalls++; return "Fallback value"; }
        );

        await Assert.That(result).IsEqualTo("Fallback value");
        await Assert.That(fallbackCalls).IsEqualTo(1);
    }
}
