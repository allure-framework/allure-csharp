using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.BddLabels;

class StoryLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.StoryAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckStoryOnTestMethodWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.Tests.Samples.StoryAttributes.OnTestMethod.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckStoryOnTestClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.Tests.Samples.StoryAttributes.OnTestClass.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckStoryOnBaseClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.Tests.Samples.StoryAttributes.OnBaseClass.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckStoryOnInterfaceWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.Tests.Samples.StoryAttributes.OnInterface.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }
}
