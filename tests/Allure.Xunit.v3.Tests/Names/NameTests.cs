using Allure.Testing;

namespace Allure.Xunit.v3.Tests.Names;

class NameTests
{
    [Test]
    public async Task CheckAllureNameOnTestMethodRenamesTest(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.NameAttributeOnMethod, token);

        await Assert.That(results).HasSingleTestResult().With.Name("Lorem Ipsum");
    }

    [Test]
    public async Task CheckAllureNameOnTestClassAffectsSubSuiteOnly(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.NameAttributeOnClass, token);

        await Assert.That(results).HasSingleTestResult()
            .With.Name("Allure.Xunit.v3.Tests.Samples.Names.NameAttributeOnClass.TestClass.TestMethod")
            .With.SingleLabel("subSuite").That.HasValue("Lorem Ipsum");
    }

    [Test]
    public async Task CheckXunitDisplayNameOnFactRenamesTest(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.XunitDisplayNameOnFact, token);

        await Assert.That(results).HasSingleTestResult().With.Name("Lorem Ipsum");
    }

    [Test]
    public async Task CheckXunitDisplayNameOnTheoryRenamesTest(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.XunitDisplayNameOnTheory, token);

        await Assert.That(results).HasSingleTestResult().With.Name("Lorem Ipsum(value: \"foo\")");
    }

    [Test]
    public async Task CheckTheoryUsesMethodNameByDefault(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SingleTheory, token);

        await Assert.That(results).HasSingleTestResult().With.Name(
            "Allure.Xunit.v3.Tests.Samples.Names.SingleTheory.TestClass.TestMethod(value: \"foo\")"
        );
    }
}
