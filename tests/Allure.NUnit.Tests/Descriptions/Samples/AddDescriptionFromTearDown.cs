using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.AddDescriptionFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.SetDescription("Lorem Ipsum");
        }

        [Test]
        public void TestMethod() { }
    }
}
