using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.SuiteLabels;

class SubSuiteLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SubSuiteApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckSubSuiteOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteApi.AttributeOnTestMethod.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");

    [Test]
    public async Task CheckSubSuiteOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteApi.AttributeOnTestClass.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");

    [Test]
    public async Task CheckSubSuiteOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteApi.AttributeOnBaseClass.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");

    [Test]
    public async Task CheckSubSuiteOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteApi.AttributeOnInterface.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");

    [Test]
    public async Task CheckSyncSubSuiteApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteApi.SyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");

    [Test]
    public async Task CheckAsyncSubSuiteApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteApi.AsyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("subSuite").With.Value("Sub Suite");
}
