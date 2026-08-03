using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Display;

public class SetFixtureNameTests
{
    [Test]
    public async Task SetFixtureNameRenamesCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            AllureInProcessApi.SetUp(
                "fixture",
                _ => AllureApi.SetFixtureName("renamed")
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task SetFixtureNameAsyncRenamesCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                (_, _) => AllureApi.SetFixtureNameAsync(
                    "renamed",
                    CancellationToken.None
                ),
                CancellationToken.None
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task SetFixtureNameThrowsIfNoFixtureRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.Run(_ => AllureApi.SetFixtureName("never"))
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetFixtureNameAsyncThrowsIfNoFixtureRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.RunAsync(
                _ => AllureApi.SetFixtureNameAsync(
                    "never",
                    CancellationToken.None
                )
            )
        ).Throws<InvalidOperationException>();
    }

    static TestResultScope NewScope() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "scope",
    };
}
