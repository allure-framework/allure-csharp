using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.CustomLabels.Samples.LegacyLabelAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureLabel("foo", "bar")]
        public void TestMethod() { }
    }
}
