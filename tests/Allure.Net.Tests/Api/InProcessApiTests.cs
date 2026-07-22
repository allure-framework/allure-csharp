using Allure.Abstractions;

namespace Allure.Net.Tests.Api;

using AllureTestResult = Allure.Model.TestResult;

public class InProcessApiTests : AllureApiTestsBase
{
    [Test]
    public async Task UpdateForwardsDelegateToInProcessOperations()
    {
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        Action<AllureTestResult> update = result => result.Name = "updated";

        AllureInProcessApi.UpdateTestResult(update);

        await Assert.That(endpoint.SyncApi.UpdateTestResult(update)).WasCalled(Times.Once);
    }

    [Test]
    public async Task ReadReturnsValueFromInProcessOperations()
    {
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TryReadTestResult(Any<Func<AllureTestResult, string>>())
            .SetsOutResult("runtime value")
            .Returns(true);

        var result = AllureInProcessApi.ReadTestResult(test => test.Name);

        await Assert.That(endpoint.SyncApi.TryReadTestResult(Any<Func<AllureTestResult, string>>()))
            .WasCalled(Times.Once);
        await Assert.That(result).IsEqualTo("runtime value");
    }
}
