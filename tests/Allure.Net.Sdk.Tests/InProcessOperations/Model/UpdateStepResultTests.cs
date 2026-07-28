using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Model;

public class UpdateStepResultTests
{
    [Test]
    public async Task UpdateStepResultUpdatesCurrentStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step("step", _ =>
                AllureInProcessApi.UpdateStepResult(
                    result => result.Name = "updated"
                )
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("updated");
    }

    [Test]
    public async Task UpdateStepResultThrowsIfNoStepRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.Run(
            _ => AllureInProcessApi.UpdateStepResult(_ => { })
        )).Throws<InvalidOperationException>();
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
