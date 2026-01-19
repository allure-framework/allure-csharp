using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    [AllureSubSuite("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
