using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddDescriptionFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.SetDescription("Lorem Ipsum");
        }

        [Test]
        public void TestMethod() { }
    }
}
