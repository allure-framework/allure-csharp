using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddEpicFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.AddEpic("foo");
        }

        [Test]
        public void TestMethod() { }
    }
}

