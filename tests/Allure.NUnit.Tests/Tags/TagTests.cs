using Allure.Testing;

namespace Allure.NUnit.Tests.Tags;

class TagTests
{
    [Test]
    public async Task AddTagsApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.AddTagsApiCalls);

        await Assert.That(results).HasSingleTestResult()
            .With.Label(l => l.HasName("tag").And.HasValue("foo"))
            .With.Label(l => l.HasName("tag").And.HasValue("bar"))
            .With.Label(l => l.HasName("tag").And.HasValue("baz"))
            .With.Label(l => l.HasName("tag").And.HasValue("qux"));
    }

    [Test]
    public async Task TagAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.TagAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.Label(l => l.HasName("tag").And.HasValue("foo"))
            .With.Label(l => l.HasName("tag").And.HasValue("bar"))
            .With.Label(l => l.HasName("tag").And.HasValue("baz"))
            .With.Label(l => l.HasName("tag").And.HasValue("qux"))
            .With.Label(l => l.HasName("tag").And.HasValue("qut"));
    }

    [Test]
    public async Task LegacyTagAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LegacyTagAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.Label(l => l.HasName("tag").And.HasValue("foo"))
            .With.Label(l => l.HasName("tag").And.HasValue("bar"))
            .With.Label(l => l.HasName("tag").And.HasValue("baz"))
            .With.Label(l => l.HasName("tag").And.HasValue("qux"));
    }

    [Test]
    public async Task NUnitCategoriesAreConvertedToTags()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.NUnitCategoryAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.Label(l => l.HasName("tag").And.HasValue("foo"))
            .With.Label(l => l.HasName("tag").And.HasValue("bar"));
    }
}
