using Allure.Abstractions;
using TUnit.Mocks.Assertions;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Operations.Model;

public class UpdateTestResultTests : AllureApiTestsBase
{
    [Test]
    public async Task UpdateTestResultRoutedToEndpoint()
    {
        Action<TestResult> update = result => result.Name = "Updated";
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);

        AllureInProcessApi.UpdateTestResult(update);

        await Assert.That(endpoint.SyncApi.UpdateTestResult(update))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void UpdateTestResultDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureInProcessApi.UpdateTestResult(_ => { });
    }
}
