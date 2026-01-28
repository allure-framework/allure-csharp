using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.BddHierarchyAttributeOnClass
{
    [AllureNUnit]
    [AllureBddHierarchy("foo", "bar", "baz")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
