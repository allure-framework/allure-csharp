using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Globals;

public class AddGlobalErrorTests
{
    [Test]
    public async Task AddGlobalErrorWritesStatusDetails()
    {
        var environment = AllureApiTestEnvironment.Create();
        var details = NewDetails();

        var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        environment.Run(_ => AllureApi.AddGlobalError(details));
        var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var global = await Assert.That(environment.Destination.Globals).HasSingleItem();
        var error = await Assert.That(global.Errors).HasSingleItem();
        await AssertError(error, before, after);
    }

    [Test]
    public async Task AddGlobalErrorAsyncWritesStatusDetails()
    {
        var environment = AllureApiTestEnvironment.Create();
        var details = NewDetails();

        var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await environment.RunAsync(_ => AllureApi.AddGlobalErrorAsync(
            details,
            CancellationToken.None
        ));
        var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var global = await Assert.That(environment.Destination.Globals).HasSingleItem();
        var error = await Assert.That(global.Errors).HasSingleItem();
        await AssertError(error, before, after);
    }

    static StatusDetails NewDetails() => new()
    {
        Message = "failure",
        Trace = "trace",
        Flaky = true,
        Known = true,
        Muted = true,
    };

    static async Task AssertError(GlobalError error, long before, long after)
    {
        await Assert.That(error.Message).IsEqualTo("failure");
        await Assert.That(error.Trace).IsEqualTo("trace");
        await Assert.That(error.Flaky).IsTrue();
        await Assert.That(error.Known).IsTrue();
        await Assert.That(error.Muted).IsTrue();
        await Assert.That(error.Timestamp).IsBetween(before, after);
    }
}
