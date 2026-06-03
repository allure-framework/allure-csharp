using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.NUnitDescriptionPropertyOnTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test(Description = "Lorem Ipsum")]
        public void TestMethod() { }
    }
}
