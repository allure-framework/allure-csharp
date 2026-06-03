using Allure.Testing;

namespace Allure.Xunit.Tests.Tags;

class TagTests
{
    [Test]
    public async Task AddTagsApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddTagsApiCalls);

        await Assert.That(results).HasSingleTestResult()
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("foo"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("bar"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("baz"));
    }

    [Test]
    public async Task TagAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.TagAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("foo"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("bar"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("baz"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("qux"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("qut"));
    }

    [Test]
    public async Task LegacyTagAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyTagAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("bar"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("baz"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("qux"));
    }
}
