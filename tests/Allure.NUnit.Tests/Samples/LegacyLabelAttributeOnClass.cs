using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LabelAttributeOnClass
{
    [AllureNUnit]
    [AllureLabel("foo", "bar")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}

