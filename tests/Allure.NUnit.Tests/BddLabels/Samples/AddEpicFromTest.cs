using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.AddEpicFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.AddEpic("foo");
        }
    }
}
