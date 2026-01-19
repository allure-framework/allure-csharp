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
            AllureApi.AddTestParameter("foo", "bar");
        }

        [Test]
        public void TestMethod() { }
    }
}
