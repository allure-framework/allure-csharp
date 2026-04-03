using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.AllureIds.Samples.SetAllureIdFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.SetAllureId(1001);
        }

        [Test]
        public void TestMethod() { }
    }
}
