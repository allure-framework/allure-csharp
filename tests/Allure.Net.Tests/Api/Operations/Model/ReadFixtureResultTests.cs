using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Model;

public class ReadFixtureResultTests : AllureApiTestsBase
{
    [Test]
    public async Task ReadFixtureResultReturnsEndpointValue()
    {
        Func<FixtureResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadFixtureResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadFixtureResult(read);

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadFixtureResult<string>(read))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ReadFixtureResultThrowsWhenValueIsMissing()
    {
        Func<FixtureResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadFixtureResult<string>(read).Returns(false);

        await Assert.That(() => AllureInProcessApi.ReadFixtureResult(read))
            .Throws<InvalidOperationException>()
            .WithMessage("Cannot read fixture result: no fixture is currently running.");
    }

    [Test]
    public async Task ReadFixtureResultWithFallbackReturnsEndpointValue()
    {
        Func<FixtureResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadFixtureResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadFixtureResult(read, "Fallback value");

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadFixtureResult<string>(read))
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task ReadFixtureResultWithFallbackReturnsFallbackWhenValueIsMissing()
    {
        Func<FixtureResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadFixtureResult<string>(read).Returns(false);

        var result = AllureInProcessApi.ReadFixtureResult(read, "Fallback value");

        await Assert.That(result).IsEqualTo("Fallback value");
    }

    [Test]
    public async Task ReadFixtureResultWithFactoryDoesNotInvokeFactoryWhenValueExists()
    {
        Func<FixtureResult, string> read = result => result.Name;
        var fallbackCalls = 0;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadFixtureResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadFixtureResult(
            read,
            () => { fallbackCalls++; return "Fallback value"; }
        );

        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(fallbackCalls).IsZero();
    }

    [Test]
    public async Task ReadFixtureResultWithFactoryInvokesFactoryWhenValueIsMissing()
    {
        Func<FixtureResult, string> read = result => result.Name;
        var fallbackCalls = 0;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadFixtureResult<string>(read).Returns(false);

        var result = AllureInProcessApi.ReadFixtureResult(
            read,
            () => { fallbackCalls++; return "Fallback value"; }
        );

        await Assert.That(result).IsEqualTo("Fallback value");
        await Assert.That(fallbackCalls).IsEqualTo(1);
    }
}
