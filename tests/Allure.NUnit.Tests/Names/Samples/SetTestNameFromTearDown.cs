using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Names.Samples.SetTestNameFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.SetTestName("Lorem Ipsum");
        }

        [Test]
        public void TestMethod() { }
    }
}
