using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.BddLabels;

class FeatureLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.FeatureApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckFeatureOnTestMethodWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi.AttributeOnTestMethod.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckFeatureOnTestClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi.AttributeOnTestClass.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckFeatureOnBaseClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi.AttributeOnBaseClass.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckFeatureOnInterfaceWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi.AttributeOnInterface.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckSyncFeatureApiCallFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi.SyncCallFromMethod.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckAsyncFeatureApiCallFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.FeatureApi.AsyncCallFromMethod.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }
}
