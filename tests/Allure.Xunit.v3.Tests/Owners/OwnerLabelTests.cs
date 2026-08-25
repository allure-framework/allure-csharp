using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Owners;

class OwnerLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.OwnerApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckOwnerOnTestMethodWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi.AttributeOnTestMethod.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");

    [Test]
    public async Task CheckOwnerOnTestClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi.AttributeOnTestClass.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");

    [Test]
    public async Task CheckOwnerOnBaseClassWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi.AttributeOnBaseClass.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");

    [Test]
    public async Task CheckOwnerOnInterfaceWorks() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi.AttributeOnInterface.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");

    [Test]
    public async Task CheckSyncOwnerApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi.SyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");

    [Test]
    public async Task CheckAsyncOwnerApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi.AsyncCallFromMethod.TestMethod"
        ).That.HasSingleLabel("owner").With.Value("John Doe");
}
