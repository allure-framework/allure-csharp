using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.CustomLabels.Samples.AddLabelFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.AddLabel("foo", "bar");
        }
    }
}
