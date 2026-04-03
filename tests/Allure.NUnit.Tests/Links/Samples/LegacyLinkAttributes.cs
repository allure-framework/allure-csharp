using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Links.Samples.LegacyLinkAttributes
{
    [AllureLink("url-1")]
    public class TestClassBase { }

    [AllureNUnit]
    [AllureLink("name-2", "url-2")]
    public class TestsClass : TestClassBase
    {
        [Test]
        [AllureLink("name-3", "url-3")]
        public void TestMethod() { }
    }
}
