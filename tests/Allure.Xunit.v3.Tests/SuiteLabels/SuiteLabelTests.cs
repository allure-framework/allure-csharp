using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.SuiteLabels;

class SuiteLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SuiteAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckSuiteOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteAttributes.OnTestMethod.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");

    [Test]
    public async Task CheckSuiteOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteAttributes.OnTestClass.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");

    [Test]
    public async Task CheckSuiteOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteAttributes.OnBaseClass.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");

    [Test]
    public async Task CheckSuiteOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteAttributes.OnInterface.TestMethod"
        ).That.HasSingleLabel("suite").With.Value("Suite");
}
