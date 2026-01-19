using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureEpic("foo")]
    public class BaseClass {}

    [AllureNUnit]
    public class TestsClass : BaseClass
    {
        [Test]
        public void TestMethod() { }
    }
}
