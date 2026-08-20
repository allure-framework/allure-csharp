using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.CustomLabels;

class LabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LabelApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckCustomLabelOnTestMethodWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi.AttributeOnTestMethod.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }

    [Test]
    public async Task CheckCustomLabelOnTestClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi.AttributeOnTestClass.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }

    [Test]
    public async Task CheckCustomLabelOnBaseClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi.AttributeOnBaseClass.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }

    [Test]
    public async Task CheckCustomLabelOnInterfaceWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi.AttributeOnInterface.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }

    [Test]
    public async Task CheckSyncLabelApiCallFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi.SyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }

    [Test]
    public async Task CheckAsyncLabelApiCallFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelApi.AsyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }
}
