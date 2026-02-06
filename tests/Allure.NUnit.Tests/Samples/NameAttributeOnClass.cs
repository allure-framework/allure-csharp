using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyNameAttribute
{
    [AllureNUnit]
    [AllureName("Lorem Ipsum")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
