using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.SuiteLabels;

class SuiteLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SuiteApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckSuiteOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteApi.AttributeOnTestMethod.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");

    [Test]
    public async Task CheckSuiteOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteApi.AttributeOnTestClass.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");

    [Test]
    public async Task CheckSuiteOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteApi.AttributeOnBaseClass.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");

    [Test]
    public async Task CheckSuiteOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteApi.AttributeOnInterface.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");

    [Test]
    public async Task CheckSyncSuiteApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteApi.SyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");

    [Test]
    public async Task CheckAsyncSuiteApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteApi.AsyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");
}
