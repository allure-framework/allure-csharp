using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.CustomLabels.Samples.LabelAttributeOnClass
{
    [AllureNUnit]
    [AllureLabel("foo", "bar")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
