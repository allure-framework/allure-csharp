using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.NUnitDescriptionPropertyWithAllureDescriptionHtml
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test(Description = "Dolor Sit Amet")]
        public void TestMethod()
        {
            AllureApi.SetDescriptionHtml("Lorem Ipsum");
        }
    }
}
