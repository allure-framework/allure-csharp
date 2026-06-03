using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.NUnitDescriptionPropertyComposition
{
    [AllureNUnit]
    [TestFixture(Description = "Lorem Ipsum")]
    public class TestsClass
    {
        [Test(Description = "Dolor Sit Amet")]
        [TestCase(1, Description = "Consectetur Adipiscing Elit")]
        public void TestMethod(int _) { }
    }
}
