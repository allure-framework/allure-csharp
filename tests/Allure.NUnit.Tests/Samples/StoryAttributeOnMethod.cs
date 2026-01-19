using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureStory("foo")]
        public void TestMethod() { }
    }
}
