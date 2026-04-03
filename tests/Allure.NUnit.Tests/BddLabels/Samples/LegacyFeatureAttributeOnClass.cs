using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.LegacyFeatureAttributeOnClass
{
    [AllureNUnit]
    [AllureFeature("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
