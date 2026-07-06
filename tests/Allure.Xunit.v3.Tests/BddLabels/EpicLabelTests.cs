using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.BddLabels;

class EpicLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.EpicAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckEpicOnTestMethodWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes.OnTestMethod.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckEpicOnTestClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes.OnTestClass.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckEpicOnBaseClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes.OnBaseClass.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckEpicOnInterfaceWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.EpicLabels.EpicAttributes.OnInterface.TestMethod"
        ).With.SingleLabel("epic").That.HasValue("Foo");
    }
}
