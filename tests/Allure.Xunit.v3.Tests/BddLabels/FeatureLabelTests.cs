using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.BddLabels;

class FeatureLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.FeatureAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckFeatureOnTestMethodWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.FeatureLabels.FeatureAttributes.OnTestMethod.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckFeatureOnTestClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.FeatureLabels.FeatureAttributes.OnTestClass.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckFeatureOnBaseClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.FeatureLabels.FeatureAttributes.OnBaseClass.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckFeatureOnInterfaceWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.FeatureLabels.FeatureAttributes.OnInterface.TestMethod"
        ).With.SingleLabel("feature").That.HasValue("Foo");
    }
}
