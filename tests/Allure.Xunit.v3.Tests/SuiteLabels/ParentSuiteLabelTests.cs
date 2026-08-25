using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.SuiteLabels;

class ParentSuiteLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.ParentSuiteApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckParentSuiteOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi.AttributeOnTestMethod.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");

    [Test]
    public async Task CheckParentSuiteOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi.AttributeOnTestClass.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");

    [Test]
    public async Task CheckParentSuiteOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi.AttributeOnBaseClass.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");

    [Test]
    public async Task CheckParentSuiteOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi.AttributeOnInterface.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");

    [Test]
    public async Task CheckSyncParentSuiteApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi.SyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");

    [Test]
    public async Task CheckAsyncParentSuiteApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi.AsyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("parentSuite").With.Value("Parent Suite");
}
