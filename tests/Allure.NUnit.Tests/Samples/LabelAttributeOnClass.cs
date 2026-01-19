using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    [AllureLabel("foo", "bar")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
