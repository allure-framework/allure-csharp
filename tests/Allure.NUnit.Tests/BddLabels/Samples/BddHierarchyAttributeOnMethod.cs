using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.BddLabels.Samples.BddHierarchyAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureBddHierarchy("foo", "bar", "baz")]
        public void TestMethod() { }
    }
}
