using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Tags;

class TagLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.TagApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckTagsOnTestMethodWork() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagApi.AttributeOnTestMethod.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));

    [Test]
    public async Task CheckTagsOnTestClassWork() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagApi.AttributeOnTestClass.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));

    [Test]
    public async Task CheckTagsOnBaseClassWork() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagApi.AttributeOnBaseClass.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));

    [Test]
    public async Task CheckTagsOnInterfaceWork() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagApi.AttributeOnInterface.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));

    [Test]
    public async Task CheckSyncTagApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagApi.SyncCallFromMethod.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));

    [Test]
    public async Task CheckAsyncTagApiCallFromMethod() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagApi.AsyncCallFromMethod.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));
}
