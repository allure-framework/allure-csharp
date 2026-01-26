using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.EpicAttributeOnBaseClass
{
    [AllureEpic("foo")]
    public class BaseClass {}

    [AllureNUnit]
    public class TestsClass : BaseClass
    {
        [Test]
        public void TestMethod() { }
    }
}

