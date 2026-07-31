using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Labels;

public class AddLabelTests
{
    [Test]
    public async Task AddLabelAddsLabelToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        var label = new Label { Name = "name", Value = "value" };

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddLabel(label);
        });

        await Assert.That(test.Labels.Single()).IsSameReferenceAs(label);
    }

    [Test]
    public async Task AddLabelAsyncAddsLabelToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        var label = new Label { Name = "name", Value = "value" };

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.AddLabelAsync(
                label,
                CancellationToken.None
            );
        });

        await Assert.That(test.Labels.Single()).IsSameReferenceAs(label);
    }

    [Test]
    public async Task AddLabelThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.Run(
            _ => AllureApi.AddLabel(
                new Label { Name = "name", Value = "value" }
            )
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddLabelAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.AddLabelAsync(
                new Label { Name = "name", Value = "value" },
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
