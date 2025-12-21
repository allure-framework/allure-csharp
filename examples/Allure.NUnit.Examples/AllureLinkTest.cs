using Allure.Net.Commons;
using Allure.NUnit.Attributes;
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
            AllureApi.AddIssue("ISSUE-123", "123");
            AllureApi.AddTmsItem("TMS-123", "123");
            AllureApi.AddLink("GitHub", "https://github.com");
        }
    }
}