using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Model;

public class UpdateTestResultTests
{
    [Test]
    public async Task UpdateTestResultUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.UpdateTestResult(
                result => result.Name = "updated"
            );
        });

        await Assert.That(test.Name).IsEqualTo("updated");
    }

    [Test]
    public async Task UpdateTestResultThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.Run(
            _ => AllureInProcessApi.UpdateTestResult(_ => { })
        )).Throws<InvalidOperationException>();
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
