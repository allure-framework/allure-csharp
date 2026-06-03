using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.SuiteHierarchyAttributeOnClass
{
    [AllureNUnit]
    [AllureSuiteHierarchy("foo", "bar", "baz")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
