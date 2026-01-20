using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddSuiteFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddSuite("foo");
        }

        [Test]
        public void TestMethod() { }
    }
}

