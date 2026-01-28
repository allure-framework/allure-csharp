using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyDescriptionAttribute
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureDescription("Lorem Ipsum")]
        public void TestMethod() { }
    }
}
