using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Links.LinkAttributes
{
    [AllureLink("url-1")]
    [AllureIssue("ISSUE-2", Title = "Issue 2")]
    [AllureTmsItem("TMS-3", Title = "TMS 3")]
    public interface IInterface { }

    [AllureLink("url-4", Title = "Link 4")]
    [AllureIssue("ISSUE-5", Title = "Issue 5")]
    [AllureTmsItem("TMS-6", Title = "TMS 6")]
    public class BaseClass { }

    [AllureLink("url-7", Type = "custom")]
    [AllureIssue("ISSUE-8")]
    [AllureTmsItem("TMS-9")]
    public class TestClass : BaseClass, IInterface
    {
        [Fact]
        [AllureLink("url-10", Title = "Link 10", Type = "custom")]
        [AllureIssue("ISSUE-11")]
        [AllureTmsItem("TMS-12")]
        public void TestMethod() { }
    }
}
