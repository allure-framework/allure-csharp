using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.FeatureAttributeOnClass
{
    [AllureNUnit]
    [AllureFeature("foo")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
