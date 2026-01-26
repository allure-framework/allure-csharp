using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.FeatureAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureFeature("foo")]
        public void TestMethod() { }
    }
}

