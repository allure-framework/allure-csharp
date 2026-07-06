using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.SuiteLabels;

class SuiteHierarchyLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SuiteHierarchyAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckSuiteHierarchyOnTestMethodWorks()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteHierarchyAttributes.OnTestMethod.TestMethod"
        );

        await Assert.That(testResult).HasSingleLabel("parentSuite").With.Value("Parent Suite");
        await Assert.That(testResult).HasSingleLabel("suite").With.Value("Suite");
        await Assert.That(testResult).HasSingleLabel("subSuite").With.Value("Sub Suite");
    }

    [Test]
    public async Task CheckSuiteHierarchyOnTestClassWorks()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteHierarchyAttributes.OnTestClass.TestMethod"
        );

        await Assert.That(testResult).HasSingleLabel("parentSuite").With.Value("Parent Suite");
        await Assert.That(testResult).HasSingleLabel("suite").With.Value("Suite");
        await Assert.That(testResult).HasSingleLabel("subSuite").With.Value("Sub Suite");
    }

    [Test]
    public async Task CheckSuiteHierarchyOnBaseClassWorks()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteHierarchyAttributes.OnBaseClass.TestMethod"
        );

        await Assert.That(testResult).HasSingleLabel("parentSuite").With.Value("Parent Suite");
        await Assert.That(testResult).HasSingleLabel("suite").With.Value("Suite");
        await Assert.That(testResult).HasSingleLabel("subSuite").With.Value("Sub Suite");
    }

    [Test]
    public async Task CheckSuiteHierarchyOnInterfaceWorks()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteHierarchyAttributes.OnInterface.TestMethod"
        );

        await Assert.That(testResult).HasSingleLabel("parentSuite").With.Value("Parent Suite");
        await Assert.That(testResult).HasSingleLabel("suite").With.Value("Suite");
        await Assert.That(testResult).HasSingleLabel("subSuite").With.Value("Sub Suite");
    }
}
