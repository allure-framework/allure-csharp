using Allure.Testing;

namespace Allure.Xunit.v3.Tests.Links;

class LinkTests
{
    [Test]
    public async Task CheckLinkAttributesWork(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LinkAttributes, new()
        {
            AllureConfiguration = new
            {
                linkTemplates = new Dictionary<string, object>
                {
                    ["issue"] = new
                    {
                        urlTemplate = "https://issues.example.org/{0}",
                        nameTemplate = "{0}",
                    },
                    ["tms"] = new
                    {
                        urlTemplate = "https://tms.example.org/{0}",
                        nameTemplate = "{0}"
                    },
                    ["custom"] = new
                    {
                        urlTemplate = "https://custom.example.org/{0}"
                    },
                },
            },
        }, token);

        await Assert.That(results).HasSingleTestResult()
            .With.Links().Count().IsEqualTo(12);

        var testResult = await Assert.That(results).HasSingleTestResult();
        await Assert.That(testResult)
            .HasLink(link => link.HasUrl("url-1").And.HasNoName().And.HasNoType())
            .And.HasLink(link => link.HasUrl("https://issues.example.org/ISSUE-2").And.HasName("Issue 2").And.HasType("issue"))
            .And.HasLink(link => link.HasUrl("https://tms.example.org/TMS-3").And.HasName("TMS 3").And.HasType("tms"))
            .And.HasLink(link => link.HasUrl("url-4").And.HasName("Link 4").And.HasNoType())
            .And.HasLink(link => link.HasUrl("https://issues.example.org/ISSUE-5").And.HasName("Issue 5").And.HasType("issue"))
            .And.HasLink(link => link.HasUrl("https://tms.example.org/TMS-6").And.HasName("TMS 6").And.HasType("tms"))
            .And.HasLink(link => link.HasUrl("https://custom.example.org/url-7").And.HasNoName().And.HasType("custom"))
            .And.HasLink(link => link.HasUrl("https://issues.example.org/ISSUE-8").And.HasName("ISSUE-8").And.HasType("issue"))
            .And.HasLink(link => link.HasUrl("https://tms.example.org/TMS-9").And.HasName("TMS-9").And.HasType("tms"))
            .And.HasLink(link => link.HasUrl("https://custom.example.org/url-10").And.HasName("Link 10").And.HasType("custom"))
            .And.HasLink(link => link.HasUrl("https://issues.example.org/ISSUE-11").And.HasName("ISSUE-11").And.HasType("issue"))
            .And.HasLink(link => link.HasUrl("https://tms.example.org/TMS-12").And.HasName("TMS-12").And.HasType("tms"));
    }
}
