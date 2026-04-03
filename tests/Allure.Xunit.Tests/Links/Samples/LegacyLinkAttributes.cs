using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Links.Samples.LegacyLinkAttributes
{
    [AllureLink("url-1")]
    [AllureIssue("url-2")]
    public class BaseClass {}

    [AllureLink("name-3", "url-3")]
    [AllureIssue("name-4", "url-4")]
    public class TestsClass : BaseClass
    {
        [Fact]
        [AllureLink("url-5")]
        [AllureIssue("url-6")]
        public void TestMethod() { }
    }
}
