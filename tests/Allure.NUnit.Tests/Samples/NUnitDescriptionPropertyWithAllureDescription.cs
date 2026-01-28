using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitDescriptionPropertyWithAllureDescription
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test(Description = "Dolor Sit Amet")]
        public void TestMethod()
        {
            AllureApi.SetDescription("Lorem Ipsum");
        }
    }
}
