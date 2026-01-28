using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.SuiteHierarchyAttributeOnClass
{
    [AllureNUnit]
    [AllureSuiteHierarchy("foo", "bar", "baz")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
