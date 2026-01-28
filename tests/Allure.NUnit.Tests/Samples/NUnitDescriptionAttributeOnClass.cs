using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitDescriptionAttributeOnClass
{
    [AllureNUnit]
    [Description("Lorem Ipsum")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
