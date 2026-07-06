using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Owners;

class OwnerLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.OwnerAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckOwnerOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerAttributes.OnTestMethod.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");

    [Test]
    public async Task CheckOwnerOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerAttributes.OnTestClass.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");

    [Test]
    public async Task CheckOwnerOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerAttributes.OnBaseClass.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");

    [Test]
    public async Task CheckOwnerOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerAttributes.OnInterface.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");
}
