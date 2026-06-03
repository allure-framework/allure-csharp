using NUnit.Framework;

namespace Allure.NUnit.Tests.Descriptions.Samples.NUnitDescriptionAttributeComposition
{
    [AllureNUnit]
    [Description("Lorem Ipsum")]
    public class TestsClass
    {
        [Test]
        [Description("Dolor Sit Amet")]
        public void TestMethod() { }
    }
}
