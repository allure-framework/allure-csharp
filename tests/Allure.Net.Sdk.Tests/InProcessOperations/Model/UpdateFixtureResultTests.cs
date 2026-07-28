using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Model;

public class UpdateFixtureResultTests
{
    [Test]
    public async Task UpdateFixtureResultUpdatesCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp("fixture", _ =>
                AllureInProcessApi.UpdateFixtureResult(
                    result => result.Name = "updated"
                )
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("updated");
    }

    [Test]
    public async Task UpdateFixtureResultThrowsIfNoFixtureRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.Run(
            _ => AllureInProcessApi.UpdateFixtureResult(_ => { })
        )).Throws<InvalidOperationException>();
    }

    static TestResultScope NewScope() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "scope",
    };
}
