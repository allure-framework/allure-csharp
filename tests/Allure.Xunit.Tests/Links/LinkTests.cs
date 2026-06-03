using Allure.Testing;

namespace Allure.Xunit.Tests.Links;

class LinkTests
{
    [Test]
    public async Task LinkRuntimeApiShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LinkRuntimeApi);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-1")
                    .And.HasNoName()
                    .And.HasNoType(),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasType("type-2"),
                link => link.HasUrl("url-3")
                    .And.HasNoName()
                    .And.HasType("issue"),
                link => link.HasUrl("url-4")
                    .And.HasNoName()
                    .And.HasType("tms"),
                link => link.HasUrl("url-5")
                    .And.HasName("name-5")
                    .And.HasNoType(),
                link => link.HasUrl("url-6")
                    .And.HasName("name-6")
                    .And.HasType("issue"),
                link => link.HasUrl("url-7")
                    .And.HasName("name-7")
                    .And.HasType("tms"),
            ]);
    }

    [Test]
    public async Task LinkAttributesShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LinkAttributes);

        var links = await Assert.That(results).HasSingleTestResult()
            .With.Links().Count().IsEqualTo(12);;

        await Assert.That(links[..3])
            .HasOnlyOneItem(
                link => link.HasUrl("url-1")
                    .And.HasNoName()
                    .And.HasNoType())
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-2")
                    .And.HasNoName()
                    .And.HasType("issue"))
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-3")
                    .And.HasNoName()
                    .And.HasType("tms"));

        await Assert.That(links[3..6])
            .HasOnlyOneItem(
                link => link.HasUrl("url-4")
                    .And.HasName("name-4")
                    .And.HasNoType())
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-5")
                    .And.HasName("name-5")
                    .And.HasType("issue"))
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-6")
                    .And.HasName("name-6")
                    .And.HasType("tms"));

        await Assert.That(links[6..9])
            .HasOnlyOneItem(
                link => link.HasUrl("url-7")
                    .And.HasNoName()
                    .And.HasType("type-7"))
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-8")
                    .And.HasNoName()
                    .And.HasType("issue"))
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-9")
                    .And.HasNoName()
                    .And.HasType("tms"));

        await Assert.That(links[9..])
            .HasOnlyOneItem(
                link => link.HasUrl("url-10")
                    .And.HasName("name-10")
                    .And.HasType("type-10"))
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-11")
                    .And.HasNoName()
                    .And.HasType("issue"))
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-12")
                    .And.HasNoName()
                    .And.HasType("tms"));
    }

    [Test]
    public async Task LegacyLinkAttributesShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyLinkAttributes);

        var links = await Assert.That(results).HasSingleTestResult()
            .With.Links().Count().IsEqualTo(4);

        await Assert.That(links[..2])
            .HasOnlyOneItem(
                link => link.HasUrl("url-3")
                    .And.HasName("name-3")
                    .And.HasType("link"))
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-4")
                    .And.HasName("name-4")
                    .And.HasType("issue"));

        await Assert.That(links[2..])
            .HasOnlyOneItem(
                link => link.HasUrl("url-5")
                    .And.HasName("url-5")
                    .And.HasType("link"))
            .And.HasOnlyOneItem(
                link => link.HasUrl("url-6")
                    .And.HasName("url-6")
                    .And.HasType("issue"));
    }
}
