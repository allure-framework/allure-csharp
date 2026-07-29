using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Model;

public class UpdateStepResultTests : AllureApiTestsBase
{
    [Test]
    public async Task UpdateStepResultRoutedToEndpoint()
    {
        Action<StepResult> update = result => result.Name = "Updated";
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);

        AllureInProcessApi.UpdateStepResult(update);

        await Assert.That(endpoint.SyncApi.UpdateStepResult(update))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void UpdateStepResultDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureInProcessApi.UpdateStepResult(_ => { });
    }
}
