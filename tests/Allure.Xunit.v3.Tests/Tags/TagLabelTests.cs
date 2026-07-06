using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Tags;

class TagLabelTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.TagAttributes, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(4);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckTagsOnTestMethodWork() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagAttributes.OnTestMethod.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));

    [Test]
    public async Task CheckTagsOnTestClassWork() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagAttributes.OnTestClass.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));

    [Test]
    public async Task CheckTagsOnBaseClassWork() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagAttributes.OnBaseClass.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));

    [Test]
    public async Task CheckTagsOnInterfaceWork() =>
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Tags.TagAttributes.OnInterface.TestMethod"
        )
            .That.HasLabel(l => l.HasName("tag").And.HasValue("foo"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("bar"))
            .And.HasLabel(l => l.HasName("tag").And.HasValue("baz"));
}
