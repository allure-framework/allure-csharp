using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.SuiteLabels;

class ParentSuiteLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.ParentSuiteAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckParentSuiteOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteAttributes.OnTestMethod.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");

    [Test]
    public async Task CheckParentSuiteOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteAttributes.OnTestClass.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");

    [Test]
    public async Task CheckParentSuiteOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteAttributes.OnBaseClass.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");

    [Test]
    public async Task CheckParentSuiteOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteAttributes.OnInterface.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");
}
