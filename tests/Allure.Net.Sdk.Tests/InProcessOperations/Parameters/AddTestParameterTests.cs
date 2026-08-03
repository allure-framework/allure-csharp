using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Parameters;

public class AddTestParameterTests
{
    [Test]
    public async Task AddTestParameterAddsParameterToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        var parameter = NewParameter();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddTestParameter(parameter);
        });

        await Assert.That(test.Parameters.Single())
            .IsSameReferenceAs(parameter);
    }

    [Test]
    public async Task AddTestParameterAsyncAddsParameterToCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        var parameter = NewParameter();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.AddTestParameterAsync(
                parameter,
                CancellationToken.None
            );
        });

        await Assert.That(test.Parameters.Single())
            .IsSameReferenceAs(parameter);
    }

    [Test]
    public async Task AddTestParameterThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.Run(
            _ => AllureApi.AddTestParameter(NewParameter())
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddTestParameterAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.AddTestParameterAsync(
                NewParameter(),
                CancellationToken.None
            )
        )).Throws<InvalidOperationException>();
    }

    static Parameter NewParameter() => new()
    {
        Name = "parameter",
        Value = "value",
        Mode = ParameterMode.Masked,
        Excluded = true,
    };

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
