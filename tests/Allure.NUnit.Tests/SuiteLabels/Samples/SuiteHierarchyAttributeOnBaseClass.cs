using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.SuiteLabels.Samples.SuiteHierarchyAttributeOnBaseClass
{
    [AllureSuiteHierarchy("foo", "bar", "baz")]
    public class BaseClass {}

    [AllureNUnit]
    public class TestsClass : BaseClass
    {
        [Test]
        public void TestMethod() { }
    }
}
