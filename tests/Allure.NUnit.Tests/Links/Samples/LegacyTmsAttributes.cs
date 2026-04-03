using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Links.Samples.LegacyTmsAttributes
{
    [AllureTms("url-1")]
    public class TestClassBase { }

    [AllureNUnit]
    [AllureTms("name-2", "url-2")]
    public class TestsClass : TestClassBase
    {
        [Test]
        [AllureTms("url-3")]
        public void TestMethod() { }
    }
}
