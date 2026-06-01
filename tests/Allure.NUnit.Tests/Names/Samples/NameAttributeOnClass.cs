using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Names.Samples.NameAttributeOnClass
{
    [AllureNUnit]
    [AllureName("Lorem Ipsum")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
