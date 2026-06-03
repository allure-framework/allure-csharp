using NUnit.Framework;

namespace Allure.NUnit.Tests.Tags.Samples.NUnitCategoryAttributes
{
    [AllureNUnit]
    [Category("foo")]
    public class TestsClass
    {
        [Test]
        [Category("bar")]
        public void TestMethod() { }
    }
}
