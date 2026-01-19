using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.AddParentSuite("foo");
        }

        [Test]
        public void TestMethod() { }
    }
}
