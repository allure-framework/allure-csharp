using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.LegacyFeatureAttributeOnBaseClass
{
    [AllureFeature("foo")]
    public class BaseClass {}

    [AllureNUnit]
    public class TestsClass : BaseClass
    {
        [Test]
        public void TestMethod() { }
    }
}
