using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitDescriptionPropertyComposition
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
