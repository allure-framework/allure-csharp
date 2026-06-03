using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Names.Samples.SetTestNameFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.SetTestName("Lorem Ipsum");
        }
    }
}
