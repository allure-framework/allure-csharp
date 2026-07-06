using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.SuiteLabels;

class SubSuiteLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SubSuiteAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckSubSuiteOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteAttributes.OnTestMethod.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");

    [Test]
    public async Task CheckSubSuiteOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteAttributes.OnTestClass.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");

    [Test]
    public async Task CheckSubSuiteOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteAttributes.OnBaseClass.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");

    [Test]
    public async Task CheckSubSuiteOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteAttributes.OnInterface.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");
}
