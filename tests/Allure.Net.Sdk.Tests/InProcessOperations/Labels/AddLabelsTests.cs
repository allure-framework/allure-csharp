using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Labels;

public class AddLabelsTests
{
    [Test]
    public async Task AddLabelsAddsLabelsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        Label[] labels =
        [
            new() { Name = "first", Value = "one" },
            new() { Name = "second", Value = "two" },
        ];

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddLabels(labels);
        });

        await Assert.That(test.Labels).IsEquivalentTo(labels);
    }

    [Test]
    public async Task AddLabelsAsyncAddsLabelsToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        Label[] labels =
        [
            new() { Name = "first", Value = "one" },
            new() { Name = "second", Value = "two" },
        ];

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.AddLabelsAsync(
                labels,
                CancellationToken.None
            );
        });

        await Assert.That(test.Labels).IsEquivalentTo(labels);
    }

    [Test]
    public async Task AddLabelsThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.Run(
            _ => AllureApi.AddLabels(
                new Label { Name = "name", Value = "value" }
            )
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddLabelsAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.AddLabelsAsync(
                [new Label { Name = "name", Value = "value" }],
                CancellationToken.None
            )
        )).Throws<InvalidOperationException>();
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
