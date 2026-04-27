using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LinkAttributes
{
    [AllureLink("url-1")]
    [AllureIssue("url-2")]
    [AllureTmsItem("url-3")]
    public interface IMetadataInterface {}

    [AllureLink("url-4", Title = "name-4")]
    [AllureIssue("url-5", Title = "name-5")]
    [AllureTmsItem("url-6", Title = "name-6")]
    public class BaseClass {}

    [AllureLink("url-7", Type = "type-7")]
    [AllureIssue("url-8")]
    [AllureTmsItem("url-9")]
    public class TestsClass : BaseClass, IMetadataInterface
    {
        [Fact]
        [AllureLink("url-10", Title = "name-10", Type = "type-10")]
        [AllureIssue("url-11")]
        [AllureTmsItem("url-12")]
        public void TestMethod() { }
    }
}



