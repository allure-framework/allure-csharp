using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.CustomLabels;

class LabelAttributeTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LabelAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckCustomLabelOnTestMethodWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelAttributes.OnTestMethod.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }

    [Test]
    public async Task CheckCustomLabelOnTestClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelAttributes.OnTestClass.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }

    [Test]
    public async Task CheckCustomLabelOnBaseClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelAttributes.OnBaseClass.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }

    [Test]
    public async Task CheckCustomLabelOnInterfaceWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.CustomLabels.LabelAttributes.OnInterface.TestMethod"
        ).That.HasSingleLabel("foo").With.Value("bar");
    }
}
