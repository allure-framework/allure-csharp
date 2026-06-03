using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.SuiteHierarchyAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureSuiteHierarchy("foo", "bar", "baz")]
        public void TestMethod() { }
    }
}
