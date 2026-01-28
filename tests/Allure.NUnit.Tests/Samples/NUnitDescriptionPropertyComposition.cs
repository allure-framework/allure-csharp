using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitDescriptionPropertyComposition
{
    [AllureNUnit]
    [TestFixture(Description = "Lorem Ipsum")]
    public class TestsClass
    {
        [Test(Description = "Dolor Sit Amet")]
        public void TestMethod() { }
    }
}
