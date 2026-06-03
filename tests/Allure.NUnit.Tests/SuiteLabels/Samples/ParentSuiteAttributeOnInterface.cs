using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.ParentSuiteAttributeOnInterface
{
    [AllureParentSuite("foo")]
    public interface IMetadataInterface {}

    [AllureNUnit]
    public class TestsClass : IMetadataInterface
    {
        [Test]
        public void TestMethod() { }
    }
}
