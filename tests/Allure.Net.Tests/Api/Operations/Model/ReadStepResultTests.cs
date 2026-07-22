using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Model;

public class ReadStepResultTests : AllureApiTestsBase
{
    [Test]
    public async Task ReadStepResultReturnsEndpointValue()
    {
        Func<StepResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadStepResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadStepResult(read);

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadStepResult<string>(read))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ReadStepResultThrowsWhenValueIsMissing()
    {
        Func<StepResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadStepResult<string>(read).Returns(false);

        await Assert.That(() => AllureInProcessApi.ReadStepResult(read))
            .Throws<InvalidOperationException>()
            .WithMessage("Cannot read step result: no step is currently running.");
    }

    [Test]
    public async Task ReadStepResultWithFallbackReturnsEndpointValue()
    {
        Func<StepResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadStepResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadStepResult(read, "Fallback value");

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadStepResult<string>(read))
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task ReadStepResultWithFallbackReturnsFallbackWhenValueIsMissing()
    {
        Func<StepResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadStepResult<string>(read).Returns(false);

        var result = AllureInProcessApi.ReadStepResult(read, "Fallback value");

        await Assert.That(result).IsEqualTo("Fallback value");
    }

    [Test]
    public async Task ReadStepResultWithFactoryDoesNotInvokeFactoryWhenValueExists()
    {
        Func<StepResult, string> read = result => result.Name;
        var fallbackCalls = 0;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadStepResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadStepResult(
            read,
            () => { fallbackCalls++; return "Fallback value"; }
        );

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(fallbackCalls).IsZero();
    }

    [Test]
    public async Task ReadStepResultWithFactoryInvokesFactoryWhenValueIsMissing()
    {
        Func<StepResult, string> read = result => result.Name;
        var fallbackCalls = 0;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadStepResult<string>(read).Returns(false);

        var result = AllureInProcessApi.ReadStepResult(
            read,
            () => { fallbackCalls++; return "Fallback value"; }
        );

        await Assert.That(result).IsEqualTo("Fallback value");
        await Assert.That(fallbackCalls).IsEqualTo(1);
    }
}
