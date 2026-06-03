using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Tags.Samples.LegacyTagAttributes
{
    [AllureTag("foo")]
    public class BaseClass { }

    [AllureNUnit]
    [AllureTag("bar")]
    public class TestsClass : BaseClass
    {
        [Test]
        [AllureTag("baz", "qux")]
        public void TestMethod() { }
    }
}
