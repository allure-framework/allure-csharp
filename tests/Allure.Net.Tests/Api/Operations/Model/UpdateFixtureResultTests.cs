using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Model;

public class UpdateFixtureResultTests : AllureApiTestsBase
{
    [Test]
    public async Task UpdateFixtureResultRoutedToEndpoint()
    {
        Action<FixtureResult> update = result => result.Name = "Updated";
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);

        AllureInProcessApi.UpdateFixtureResult(update);

        await Assert.That(endpoint.SyncApi.UpdateFixtureResult(update))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void UpdateFixtureResultDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureInProcessApi.UpdateFixtureResult(_ => { });
    }
}
