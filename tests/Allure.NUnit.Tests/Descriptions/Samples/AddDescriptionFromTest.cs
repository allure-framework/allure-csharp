using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.AddDescriptionFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.SetDescription("Lorem Ipsum");
        }
    }
}
