using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitDescriptionPropertyOnClass
{
    [AllureNUnit]
    [TestFixture(Description = "Lorem Ipsum")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
