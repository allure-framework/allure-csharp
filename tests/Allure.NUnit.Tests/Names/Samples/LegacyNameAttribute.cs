using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Names.Samples.LegacyNameAttribute
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureName("Lorem Ipsum")]
        public void TestMethod() { }
    }
}
