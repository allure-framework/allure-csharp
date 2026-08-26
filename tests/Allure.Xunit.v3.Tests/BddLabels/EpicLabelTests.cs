using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.BddLabels;

class EpicLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.EpicApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckEpicOnTestMethodWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi.AttributeOnTestMethod.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckEpicOnTestClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi.AttributeOnTestClass.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckEpicOnBaseClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi.AttributeOnBaseClass.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckEpicOnInterfaceWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi.AttributeOnInterface.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckSyncEpicApiCallFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi.SyncCallFromMethod.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckAsyncEpicApiCallFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.EpicApi.AsyncCallFromMethod.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }
}
