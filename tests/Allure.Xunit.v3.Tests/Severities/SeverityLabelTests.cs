using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Severities;

class SeverityLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SeverityAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckSeverityOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Severities.SeverityAttributes.OnTestMethod.TestMethod"
        ).That.HasSingleLabel("severity").With.Value("critical");

    [Test]
    public async Task CheckSeverityOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Severities.SeverityAttributes.OnTestClass.TestMethod"
        ).That.HasSingleLabel("severity").With.Value("critical");

    [Test]
    public async Task CheckSeverityOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Severities.SeverityAttributes.OnBaseClass.TestMethod"
        ).That.HasSingleLabel("severity").With.Value("critical");

    [Test]
    public async Task CheckSeverityOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Severities.SeverityAttributes.OnInterface.TestMethod"
        ).That.HasSingleLabel("severity").With.Value("critical");
}
