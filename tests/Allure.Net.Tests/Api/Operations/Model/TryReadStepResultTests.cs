using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Model;

public class TryReadStepResultTests : AllureApiTestsBase
{
    [Test]
    public async Task TryReadStepResultReturnsEndpointValue()
    {
        Func<StepResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadStepResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var success = AllureInProcessApi.TryReadStepResult(read, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadStepResult<string>(read))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TryReadStepResultReturnsFalseFromEndpoint()
    {
        Func<StepResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadStepResult<string?>(read)
            .SetsOutResult((string?)null)
            .Returns(false);

        var success = AllureInProcessApi.TryReadStepResult(read, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
        await Assert.That(endpoint.SyncApi.TryReadStepResult<string?>(read))
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task TryReadStepResultReturnsFalseWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var success = AllureInProcessApi.TryReadStepResult(
            result => result.Name,
            out var result
        );

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }
}
