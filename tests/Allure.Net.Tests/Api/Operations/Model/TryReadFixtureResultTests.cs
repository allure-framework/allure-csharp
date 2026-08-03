using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Model;

public class TryReadFixtureResultTests : AllureApiTestsBase
{
    [Test]
    public async Task TryReadFixtureResultReturnsEndpointValue()
    {
        Func<FixtureResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadFixtureResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var success = AllureInProcessApi.TryReadFixtureResult(read, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadFixtureResult<string>(read))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TryReadFixtureResultReturnsFalseFromEndpoint()
    {
        Func<FixtureResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadFixtureResult<string?>(read)
            .SetsOutResult(null)
            .Returns(false);

        var success = AllureInProcessApi.TryReadFixtureResult(read, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
        await Assert.That(endpoint.SyncApi.TryReadFixtureResult<string?>(read))
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task TryReadFixtureResultReturnsFalseWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var success = AllureInProcessApi.TryReadFixtureResult(
            result => result.Name,
            out var result
        );

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsDefault();
    }
}
