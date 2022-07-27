using Allure.Net.Commons;
using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Links")]
    public class AllureLinkTest : BaseTest
    {
        [Test]
        [AllureLink("GitHub", "https://github.com")]
        [AllureLink("https://google.com")]
        [AllureIssue("ISSUE-123")]
        [AllureTms("TMS-123")]
        public void StaticLinkTest()
        {
        }

        [Test]
        public void DynamicLinkTest()
        {
            AllureLifecycle.Instance.UpdateTestCase(t => t.links.Add(Link.Issue("ISSUE-123")));
            AllureLifecycle.Instance.UpdateTestCase(t => t.links.Add(Link.Tms("TMS-123")));
            AllureLifecycle.Instance.UpdateTestCase(t =>
                t.links.Add(new Link {name = "GitHub", url = "https://github.com", type = "link"}));
        }
    }
}