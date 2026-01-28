using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitDescriptionPropertyOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test(Description = "Lorem Ipsum")]
        public void TestMethod() { }
    }
}
