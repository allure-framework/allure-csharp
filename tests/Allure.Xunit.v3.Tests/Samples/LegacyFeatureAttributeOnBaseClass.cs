using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.LegacyFeatureAttributeOnBaseClass
{
    [AllureFeature("foo")]
    public class BaseClass {}

    public class TestsClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }
}



