using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.AllureIds.Samples.SetAllureIdFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.SetAllureId(1001);
        }
    }
}
