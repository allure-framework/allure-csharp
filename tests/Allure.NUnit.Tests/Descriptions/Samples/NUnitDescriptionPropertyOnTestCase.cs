using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.NUnitDescriptionPropertyOnTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [TestCase(1, Description = "Lorem Ipsum")]
        public void TestMethod(int _) { }
    }
}
