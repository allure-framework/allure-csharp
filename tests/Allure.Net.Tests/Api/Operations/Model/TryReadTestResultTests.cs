using Allure.Abstractions;
using TUnit.Mocks.Assertions;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Operations.Model;

public class TryReadTestResultTests : AllureApiTestsBase
{
    [Test]
    public async Task TryReadTestResultReturnsEndpointValue()
    {
        Func<TestResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult<string>(read)
            .SetsOutResult("Runtime value")
            .Returns(true);

        var success = AllureInProcessApi.TryReadTestResult(read, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo("Runtime value");
        await Assert.That(endpoint.SyncApi.TryReadTestResult<string>(read))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TryReadTestResultReturnsFalseFromEndpoint()
    {
        Func<TestResult, string> read = result => result.Name;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult<string?>(read)
            .SetsOutResult((string?)null)
            .Returns(false);

        var success = AllureInProcessApi.TryReadTestResult(read, out var result);

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
        await Assert.That(endpoint.SyncApi.TryReadTestResult<string?>(read))
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task TryReadTestResultReturnsFalseWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var success = AllureInProcessApi.TryReadTestResult(
            result => result.Name,
            out var result
        );

        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }
}
