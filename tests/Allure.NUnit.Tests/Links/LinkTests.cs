using Allure.Testing;

namespace Allure.NUnit.Tests.Links;

class LinkTests
{
    [Test]
    public async Task CheckLinksRuntimeApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.AddLinksAtRuntime);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-1")
                    .And.HasNoName()
                    .And.HasNoType(),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasNoType(),
                link => link.HasUrl("url-3")
                    .And.HasName("name-3")
                    .And.HasType("type-3"),
                link => link.HasUrl("url-4")
                    .And.HasName("name-4")
                    .And.HasType("type-4"),
            ]);
    }

    [Test]
    public async Task CheckLegacyLinkAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LegacyLinkAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-3")
                    .And.HasName("name-3")
                    .And.HasType("link"),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasType("link"),
                link => link.HasUrl("url-1")
                    .And.HasName("url-1")
                    .And.HasType("link"),
            ]);
    }

    [Test]
    public async Task CheckLinkAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LinkAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-1")
                    .And.HasNoName()
                    .And.HasNoType(),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasNoType(),
                link => link.HasUrl("url-3")
                    .And.HasNoName()
                    .And.HasType("type-3"),
                link => link.HasUrl("url-4")
                    .And.HasName("name-4")
                    .And.HasType("type-4"),
            ]);
    }

    [Test]
    public async Task CheckIssuesRuntimeApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.AddIssuesAtRuntime);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-1")
                    .And.HasNoName()
                    .And.HasType("issue"),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasType("issue"),
                link => link.HasUrl("url-3")
                    .And.HasName("name-3")
                    .And.HasType("issue"),
            ]);
    }

    [Test]
    public async Task CheckLegacyIssueAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LegacyIssueAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-3")
                    .And.HasName("url-3")
                    .And.HasType("issue"),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasType("issue"),
                link => link.HasUrl("url-1")
                    .And.HasName("url-1")
                    .And.HasType("issue"),
            ]);
    }

    [Test]
    public async Task CheckIssueAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.IssueAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-1")
                    .And.HasNoName()
                    .And.HasType("issue"),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasType("issue"),
                link => link.HasUrl("url-3")
                    .And.HasNoName()
                    .And.HasType("issue"),
                link => link.HasUrl("url-4")
                    .And.HasNoName()
                    .And.HasType("issue"),
            ]);
    }

    [Test]
    public async Task CheckTmsLinksRuntimeApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.AddTmsItemsAtRuntime);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-1")
                    .And.HasNoName()
                    .And.HasType("tms"),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasType("tms"),
                link => link.HasUrl("url-3")
                    .And.HasName("name-3")
                    .And.HasType("tms"),
            ]);
    }

    [Test]
    public async Task CheckLegacyTmsAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LegacyTmsAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-3")
                    .And.HasName("url-3")
                    .And.HasType("tms"),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasType("tms"),
                link => link.HasUrl("url-1")
                    .And.HasName("url-1")
                    .And.HasType("tms"),
            ]);
    }

    [Test]
    public async Task CheckTmsItemAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.TmsItemAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.LinksMatching([
                link => link.HasUrl("url-1")
                    .And.HasNoName()
                    .And.HasType("tms"),
                link => link.HasUrl("url-2")
                    .And.HasName("name-2")
                    .And.HasType("tms"),
                link => link.HasUrl("url-3")
                    .And.HasNoName()
                    .And.HasType("tms"),
                link => link.HasUrl("url-4")
                    .And.HasNoName()
                    .And.HasType("tms"),
            ]);
    }
}
