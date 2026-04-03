using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.CustomLabels.Samples.AddLabelFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.AddLabel("foo", "bar");
        }

        [Test]
        public void TestMethod() { }
    }
}
