using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.AddEpicFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddEpic("foo");
        }

        [Test]
        public void TestMethod() { }
    }
}
