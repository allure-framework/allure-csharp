using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Labels;

public class SetLabelTests
{
    [Test]
    public async Task SetLabelReplacesLabelsWithSameName()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        test.Labels.AddRange([
            Label.Owner("first"),
            Label.Owner("second"),
            Label.Tag("preserved"),
        ]);

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.SetOwner("replacement");
        });

        await Assert.That(test.Labels).IsEquivalentTo(
            [
                Label.Tag("preserved"),
                Label.Owner("replacement"),
            ]
        );
    }

    [Test]
    public async Task SetLabelAsyncReplacesLabelsWithSameName()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        test.Labels.AddRange([
            Label.Owner("first"),
            Label.Owner("second"),
            Label.Tag("preserved"),
        ]);

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.SetOwnerAsync(
                "replacement",
                CancellationToken.None
            );
        });

        await Assert.That(test.Labels).IsEquivalentTo(
            [
                Label.Tag("preserved"),
                Label.Owner("replacement"),
            ]
        );
    }

    [Test]
    public async Task SetLabelThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.Run(_ => AllureApi.SetOwner("owner"))
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetLabelAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.SetOwnerAsync(
                "owner",
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
