using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Names.Samples.SetTestNameFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.SetTestName("Lorem Ipsum");
        }

        [Test]
        public void TestMethod() { }
    }
}
