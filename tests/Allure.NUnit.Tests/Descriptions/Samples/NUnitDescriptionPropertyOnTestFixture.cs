using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.NUnitDescriptionPropertyOnTestFixture
{
    [AllureNUnit]
    [TestFixture(Description = "Lorem Ipsum")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
