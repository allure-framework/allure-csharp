using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.BddLabels;

class StoryLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.StoryApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckStoryOnTestMethodWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi.AttributeOnTestMethod.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckStoryOnTestClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi.AttributeOnTestClass.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckStoryOnBaseClassWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi.AttributeOnBaseClass.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckStoryOnInterfaceWorks()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi.AttributeOnInterface.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckSyncStoryApiCallFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi.SyncCallFromMethod.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }

    [Test]
    public async Task CheckAsyncStoryApiCallFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.BddLabels.StoryApi.AsyncCallFromMethod.TestMethod"
        ).With.SingleLabel("story").That.HasValue("Foo");
    }
}
