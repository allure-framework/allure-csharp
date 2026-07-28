using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Display;

public class SetNameTests
{
    [Test]
    public async Task SetNameRenamesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.SetName("renamed");
        });

        await Assert.That(test.Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task SetNameRenamesCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "fixture",
                _ => AllureApi.SetName("renamed")
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task SetNamePrioritizesCurrentStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                _ => AllureApi.SetName("renamed-step")
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("renamed-step");
        await Assert.That(test.Name).IsEqualTo("test");
    }

    [Test]
    public async Task SetNameAsyncRenamesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.SetNameAsync("renamed");
        });

        await Assert.That(test.Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task SetNameAsyncRenamesCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                async (_, _) => await AllureApi.SetNameAsync("renamed", CancellationToken.None),
                CancellationToken.None
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task SetNameAsyncPrioritizesCurrentStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                (_, _) => AllureApi.SetNameAsync(
                    "renamed-step",
                    CancellationToken.None
                ),
                CancellationToken.None
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("renamed-step");
        await Assert.That(test.Name).IsEqualTo("test");
    }

    [Test]
    public async Task SetNameThrowsIfNoTestStepOrFixtureRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            AllureApi.SetName("never");
        })).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetNameAsyncThrowsIfNoTestStepOrFixtureRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            await AllureApi.SetNameAsync("never");
        })).Throws<InvalidOperationException>();
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };

    static TestResultScope NewScope() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "scope",
    };
}
